using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Drawing.Design;
using System.Windows.Forms;
using System.Windows.Forms.Design;

using JetEazy.BasicSpace;
using JetEazy.UISpace;
using JetEazy;
using Allinone.BasicSpace;
using Allinone.FormSpace;
using Allinone.FormSpace.PADForm;

namespace Allinone.OPSpace.AnalyzeSpace
{
    public class ASSEMBLEClass3 : ICustomClass
    {
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

        public class GetMeasureDataPropertyEditor : UITypeEditor
        {
            public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext pContext)
            {
                if (pContext != null && pContext.Instance != null)
                {
                    //以「...」按鈕的方式顯示
                    //UITypeEditorEditStyle.DropDown    下拉選單
                    //UITypeEditorEditStyle.None        預設的輸入欄位
                    return UITypeEditorEditStyle.Modal;
                }
                return base.GetEditStyle(pContext);
            }
            public override object EditValue(ITypeDescriptorContext pContext, IServiceProvider pProvider, object pValue)
            {
                IWindowsFormsEditorService editorService = null;
                if (pContext != null && pContext.Instance != null && pProvider != null)
                {
                    editorService = (IWindowsFormsEditorService)pProvider.GetService(typeof(IWindowsFormsEditorService));
                    if (editorService != null)
                    {
                        //將顯示得視窗放在這邊，並透過ShowDialog方式來呼叫
                        //取得到的值再回傳回去
                        //MessageBox.Show("sfsf");
                        MeasureDataForm msrdataform = new MeasureDataForm((string)pValue);

                        if (msrdataform.ShowDialog() == DialogResult.OK)
                        {
                            pValue = JzToolsClass.PassingString;
                            JzToolsClass.PassingString = "";
                        }

                        //pValue = "FUCK YOU!";
                    }
                }
                return pValue;
            }
        }

        public class GetPADDataPropertyEditor : UITypeEditor
        {
            public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext pContext)
            {
                if (pContext != null && pContext.Instance != null)
                {
                    //以「...」按鈕的方式顯示
                    //UITypeEditorEditStyle.DropDown    下拉選單
                    //UITypeEditorEditStyle.None        預設的輸入欄位
                    return UITypeEditorEditStyle.Modal;
                }
                return base.GetEditStyle(pContext);
            }
            public override object EditValue(ITypeDescriptorContext pContext, IServiceProvider pProvider, object pValue)
            {
                IWindowsFormsEditorService editorService = null;
                if (pContext != null && pContext.Instance != null && pProvider != null)
                {
                    editorService = (IWindowsFormsEditorService)pProvider.GetService(typeof(IWindowsFormsEditorService));
                    if (editorService != null)
                    {
                        //將顯示得視窗放在這邊，並透過ShowDialog方式來呼叫
                        //取得到的值再回傳回去
                        //MessageBox.Show("sfsf");
                        PadInspectDataForm msrdataform = new PadInspectDataForm((string)pValue);

                        if (msrdataform.ShowDialog() == DialogResult.OK)
                        {
                            pValue = JzToolsClass.PassingString;
                            JzToolsClass.PassingString = "";
                        }

                        //pValue = "FUCK YOU!";
                    }
                }
                return pValue;
            }
        }
        public class GetNoHaveModePropertyEditor : UITypeEditor
        {
            public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext pContext)
            {
                if (pContext != null && pContext.Instance != null)
                {
                    //以「...」按鈕的方式顯示
                    //UITypeEditorEditStyle.DropDown    下拉選單
                    //UITypeEditorEditStyle.None        預設的輸入欄位
                    return UITypeEditorEditStyle.Modal;
                }
                return base.GetEditStyle(pContext);
            }
            public override object EditValue(ITypeDescriptorContext pContext, IServiceProvider pProvider, object pValue)
            {
                IWindowsFormsEditorService editorService = null;
                if (pContext != null && pContext.Instance != null && pProvider != null)
                {
                    editorService = (IWindowsFormsEditorService)pProvider.GetService(typeof(IWindowsFormsEditorService));
                    if (editorService != null)
                    {
                        //將顯示得視窗放在這邊，並透過ShowDialog方式來呼叫
                        //取得到的值再回傳回去
                        //MessageBox.Show("sfsf");
                        PadNoHaveForm msrdataform = new PadNoHaveForm((string)pValue);

                        if (msrdataform.ShowDialog() == DialogResult.OK)
                        {
                            pValue = JzToolsClass.PassingString;
                            JzToolsClass.PassingString = "";
                        }

                        //pValue = "FUCK YOU!";
                    }
                }
                return pValue;
            }
        }
        public class GetExtendPropertyEditor : UITypeEditor
        {
            public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext pContext)
            {
                if (pContext != null && pContext.Instance != null)
                {
                    //以「...」按鈕的方式顯示
                    //UITypeEditorEditStyle.DropDown    下拉選單
                    //UITypeEditorEditStyle.None        預設的輸入欄位
                    return UITypeEditorEditStyle.Modal;
                }
                return base.GetEditStyle(pContext);
            }
            public override object EditValue(ITypeDescriptorContext pContext, IServiceProvider pProvider, object pValue)
            {
                IWindowsFormsEditorService editorService = null;
                if (pContext != null && pContext.Instance != null && pProvider != null)
                {
                    editorService = (IWindowsFormsEditorService)pProvider.GetService(typeof(IWindowsFormsEditorService));
                    if (editorService != null)
                    {
                        //將顯示得視窗放在這邊，並透過ShowDialog方式來呼叫
                        //取得到的值再回傳回去
                        //MessageBox.Show("sfsf");
                        PadExtendForm msrdataform = new PadExtendForm((string)pValue);

                        if (msrdataform.ShowDialog() == DialogResult.OK)
                        {
                            pValue = JzToolsClass.PassingString;
                            JzToolsClass.PassingString = "";
                        }

                        //pValue = "FUCK YOU!";
                    }
                }
                return pValue;
            }
        }

        public PropertyList publicproperties = new PropertyList();
        //ICustomClass implementation
        public PropertyList PublicProperties
        {
            get
            {
                return publicproperties;
            }
            set
            {
                publicproperties = value;
            }
        }

        #region Normal Factors

        [Category("01.Normal"), DefaultValue(""), ReadOnly(true)]
        public virtual string Name
        {
            get; set;
        }
        [Category("01.Normal"), DefaultValue("")]
        public virtual string AliasName
        {
            get; set;
        }
        [Category("01.Normal"), DefaultValue(0)]
        [Description("0 is No Need.\rRange -200 <-> 200, default 0")]
        [TypeConverter(typeof(NumericUpDownTypeConverter))]
        [Editor(typeof(NumericUpDownTypeEditor), typeof(UITypeEditor)), MinMax(-200, 200)]
        public virtual int Brightness
        {
            get; set;
        }
        [Category("01.Normal"), DefaultValue(0)]
        [Description("0 is No Need.\rRange -200 <-> 200, default 0")]
        [TypeConverter(typeof(NumericUpDownTypeConverter))]
        [Editor(typeof(NumericUpDownTypeEditor), typeof(UITypeEditor)), MinMax(-200, 200)]
        public virtual int Contrast
        {
            get; set;
        }
        [Category("01.Normal"), DefaultValue(MaskMethodEnum.NONE)]
        [Description("Direct or Relate Mask.")]
        [DisplayName("Mask Method")]
        [TypeConverter(typeof(JzEnumConverter))]
        public virtual MaskMethodEnum MaskMethod
        {
            get; set;
        }
        [Category("01.Normal"), DefaultValue(20)]
        [Description("0 is No Need.\rRange 0 <-> 1000, default 20")]
        [TypeConverter(typeof(NumericUpDownTypeConverter))]
        [Editor(typeof(NumericUpDownTypeEditor), typeof(UITypeEditor)), MinMax(0, 1000)]
        public virtual int ExtendX
        {
            get; set;
        }
        [Category("01.Normal"), DefaultValue(20)]
        [Description("0 is No Need.\rRange 0 <-> 1000, default 20")]
        [TypeConverter(typeof(NumericUpDownTypeConverter))]
        [Editor(typeof(NumericUpDownTypeEditor), typeof(UITypeEditor)), MinMax(0, 1000)]
        public virtual int ExtendY
        {
            get; set;
        }
        [Category("01.Normal"), DefaultValue("None")]
        [Description("Relate ASN Name")]
        [Browsable(true)]
        [TypeConverter(typeof(JzASNStringConverter))]
        [DisplayName("R.ASN")]
        public virtual string RelateASN
        {
            get; set;
        }
        [Category("01.Normal"), DefaultValue("None")]
        [Description("Relate ASN Item Name")]
        [Browsable(true)]
        [TypeConverter(typeof(JzASNItemStringConverter))]
        [DisplayName("R.ASNItem")]
        public virtual string RelateASNItem
        {
            get; set;
        }
        [Category("01.Normal"), DefaultValue("false")]
        [Description("IsSeed")]
        [Browsable(true)]
        //[TypeConverter(typeof(JzASNItemStringConverter))]
        [DisplayName("Seed(种子)")]
        public virtual bool IsSeed
        {
            get; set;
        }
        #endregion

        #region Align Factor

        [Category("02.Align"), DefaultValue(AlignMethodEnum.NONE)]
        [Description("Using Align Method.")]
        [DisplayName("A.Method")]
        [TypeConverter(typeof(JzEnumConverter))]
        public virtual AlignMethodEnum AlignMethod
        {
            get; set;
        }

        [Category("02.Align"), DefaultValue(AlignModeEnum.AREA)]
        [Description("Using Align Mode.")]
        [DisplayName("A.Mode")]
        [TypeConverter(typeof(JzEnumConverter))]
        public virtual AlignModeEnum AlignMode
        {
            get; set;
        }

        [Category("02.Align"), DefaultValue(20)]
        [Description("Sampling Size, 0 is auto.\rRange 0 <-> 1000, default 35")]
        [DisplayName("S.Size")]
        [TypeConverter(typeof(NumericUpDownTypeConverter))]
        [Editor(typeof(NumericUpDownTypeEditor), typeof(UITypeEditor)), MinMax(-10f, 1000, 1f, 1)]
        public virtual float MTPSample
        {
            get; set;
        }

        [Category("02.Align"), DefaultValue(true)]
        [Description("Canny Auto , default is true")]
        [DisplayName("Canny Auto")]
        [TypeConverter(typeof(bool))]
        public virtual bool MTCannyAuto
        {
            get; set;
        }

        [Category("02.Align"), DefaultValue(200)]
        [Description("Canny H. Threshold\rRange 0 <-> 200, default 200")]
        [DisplayName("Canny H.")]
        [TypeConverter(typeof(NumericUpDownTypeConverter))]
        [Editor(typeof(NumericUpDownTypeEditor), typeof(UITypeEditor)), MinMax(0, 255)]
        public virtual int MTCannyH
        {
            get; set;
        }

        [Category("02.Align"), DefaultValue(128)]
        [Description("Canny L. Threshold.\rRange 0 <-> 200,  default 128")]
        [DisplayName("Canny L.")]
        [TypeConverter(typeof(NumericUpDownTypeConverter))]
        [Editor(typeof(NumericUpDownTypeEditor), typeof(UITypeEditor)), MinMax(0, 255)]
        public virtual int MTCannyL
        {
            get; set;
        }

        [Category("02.Align"), DefaultValue(20)]
        [Description("Found Rotation.\rRange 0 <-> 180.,  default 20.00")]
        [DisplayName("Rotation")]
        [TypeConverter(typeof(NumericUpDownTypeConverter))]
        [Editor(typeof(NumericUpDownTypeEditor), typeof(UITypeEditor)), MinMax(0f, 180f, 1f, 2)]
        public virtual float MTRotation
        {
            get; set;
        }

        [Category("02.Align"), DefaultValue(0)]
        [Description("Found Scaling.\rRange 0% <-> 100%.,  default 0.00")]
        [DisplayName("Scaling")]
        [TypeConverter(typeof(NumericUpDownTypeConverter))]
        [Editor(typeof(NumericUpDownTypeEditor), typeof(UITypeEditor)), MinMax(0f, 100f, 1f, 2)]
        public virtual float MTScaling
        {
            get; set;
        }
        [Category("02.Align"), DefaultValue(1)]
        [Description("Max Occ.\rRange 1 <-> 500,  default 1")]
        [DisplayName("Max Occ")]
        [TypeConverter(typeof(NumericUpDownTypeConverter))]
        [Editor(typeof(NumericUpDownTypeEditor), typeof(UITypeEditor)), MinMax(1, 500)]
        public virtual int MTMaxOcc
        {
            get; set;
        }
        [Category("02.Align"), DefaultValue(0.1)]
        [Description("Align Tolerance.\rRange 0 <-> 1.,  default 0.1")]
        [DisplayName("A.Tolerance")]
        [TypeConverter(typeof(NumericUpDownTypeConverter))]
        [Editor(typeof(NumericUpDownTypeEditor), typeof(UITypeEditor)), MinMax(0f, 1f, 0.1f, 2)]
        public virtual float MTTolerance
        {
            get; set;
        }
        [Category("02.Align"), DefaultValue(false)] //No Use Now
        [DisplayName("Subpixel")]
        public virtual bool MTIsSubPixel
        {
            get; set;
        }
        [Category("02.Align"), DefaultValue(0.5)]
        [Description("Offset Tolerance.\rRange 0 <-> 100.,  default 0.5")]
        [DisplayName("Offset")]
        [TypeConverter(typeof(NumericUpDownTypeConverter))]
        [Editor(typeof(NumericUpDownTypeEditor), typeof(UITypeEditor)), MinMax(0f, 100f, 0.1f, 3)]
        public virtual float MTOffset
        {
            get; set;
        }
        [Category("02.Align"), DefaultValue(0.02)]
        [Description("Dot Resolution.\rRange 0 <-> 100.,  default 0.02")]
        [DisplayName("Resolution")]
        [TypeConverter(typeof(NumericUpDownTypeConverter))]
        [Editor(typeof(NumericUpDownTypeEditor), typeof(UITypeEditor)), MinMax(0f, 100f, 0.1f, 3)]
        public virtual float MTResolution
        {
            get; set;
        }


        [Category("02.Align"), DefaultValue(AlignMethodEnum.NONE)]
        [Description("Using AbsAlign Method.")]
        [DisplayName("B.ABSMethod")]
        [TypeConverter(typeof(JzEnumConverter))]
        public virtual AbsoluteAlignEnum ABSAlignMethod
        {
            get; set;
        }
        [Category("02.Align"), DefaultValue(0.2)]
        [Description("ABSOffset Tolerance.\rRange 0 <-> 100.,  default 0.2")]
        [DisplayName("ABSOffset")]
        [TypeConverter(typeof(NumericUpDownTypeConverter))]
        [Editor(typeof(NumericUpDownTypeEditor), typeof(UITypeEditor)), MinMax(0f, 100f, 0.1f, 3)]
        public virtual float ABSOffset
        {
            get; set;
        }

        #endregion

        #region Inspection Factor

        [Category("03.Inspection"), DefaultValue(InspectionMethodEnum.NONE)]
        [Description("Using Inspection Method.")]
        [DisplayName("I.Method")]
        [TypeConverter(typeof(InspectionMethodEnum))]
        public virtual InspectionMethodEnum InspectionMethod
        {
            get; set;
        }

        [Category("03.Inspection"), DefaultValue(Inspection_A_B_Enum.AB)]
        [Description("Using Inspection A-B  Method.")]
        [DisplayName("A-B")]
        [TypeConverter(typeof(InspectionMethodEnum))]
        public virtual Inspection_A_B_Enum Inspection_A_B_Method
        {
            get; set;
        }

        [Category("03.Inspection"), DefaultValue(3)]
        [Description("Max Count\rRange -1 <-> 1000.,  default 3, -1 to Disable")]
        [DisplayName("Count")]
        [TypeConverter(typeof(NumericUpDownTypeConverter))]
        [Editor(typeof(NumericUpDownTypeEditor), typeof(UITypeEditor)), MinMax(-1, 1000)]
        public virtual int IBCount
        {
            get; set;
        }
        [Category("03.Inspection"), DefaultValue(5)]
        [Description("Max Area\rRange 1 <-> 1000.,  default 5")]
        [DisplayName("Area")]
        [TypeConverter(typeof(NumericUpDownTypeConverter))]
        [Editor(typeof(NumericUpDownTypeEditor), typeof(UITypeEditor)), MinMax(1, 1000)]
        public virtual int IBArea
        {
            get; set;
        }
        [Category("03.Inspection"), DefaultValue(15)]
        [Description("Pixel Tolerance.\rRange 0 <-> 255,  default is 15")]
        [DisplayName("I.Tolerance")]
        [TypeConverter(typeof(NumericUpDownTypeConverter))]
        [Editor(typeof(NumericUpDownTypeEditor), typeof(UITypeEditor)), MinMax(0, 255)]
        public virtual int IBTolerance
        {
            get; set;
        }


        #endregion

        #region Measure Factor
        [Category("04.Measure"), DefaultValue(MeasureMethodEnum.NONE), ReadOnly(true)]
        //[Category("04.Measure")]
        [Description("Using Measure Method.")]
        [DisplayName("M.Method")]
        public virtual MeasureMethodEnum MeasureMethod
        {
            get; set;
        }
        [Category("04.Measure"), DefaultValue(10)]
        [Description("Measure Tolerance.\rRange 0% <-> 100%.,  default is 10(%)")]
        [DisplayName("M.Tolerance")]
        [TypeConverter(typeof(NumericUpDownTypeConverter))]
        [Editor(typeof(NumericUpDownTypeEditor), typeof(UITypeEditor)), MinMax(0f, 100f, 1f, 2)]
        public virtual float MMTolerance
        {
            get; set;
        }
        [Category("04.Measure"), DefaultValue(""), ReadOnly(false)]
        [Description("Operation String")]
        [DisplayName("OP String")]
        [Editor(typeof(GetMeasureDataPropertyEditor), typeof(UITypeEditor))]
        public virtual string MMOPString
        {
            get; set;
        }
        [Category("04.Measure"), DefaultValue(5)]
        [Description("Max Gap.\rRange 0 <-> 1000.,  default is 5")]
        [DisplayName("Max Gap")]
        [TypeConverter(typeof(NumericUpDownTypeConverter))]
        [Editor(typeof(NumericUpDownTypeEditor), typeof(UITypeEditor)), MinMax(0f, 1000f, 1f, 2)]
        public virtual float MMMaxGap
        {
            get; set;
        }
        [Category("04.Measure"), DefaultValue(5)]
        [Description("Min Gap.\rRange 0 <-> 1000.,  default is 5")]
        [DisplayName("Min Gap")]
        [TypeConverter(typeof(NumericUpDownTypeConverter))]
        [Editor(typeof(NumericUpDownTypeEditor), typeof(UITypeEditor)), MinMax(0f, 1000f, 1f, 2)]
        public virtual float MMMinGap
        {
            get; set;
        }
        [Category("04.Measure"), DefaultValue(5)]
        [Description("Pixel Gap.\rRange 0 <-> 100.,  default is 5")]
        [DisplayName("Pixel Gap")]
        [TypeConverter(typeof(NumericUpDownTypeConverter))]
        [Editor(typeof(NumericUpDownTypeEditor), typeof(UITypeEditor)), MinMax(0, 100)]
        public virtual int MMPixelGap
        {
            get; set;
        }
        [Category("04.Measure"), DefaultValue(0)]
        [Description("Head Tail Ratio.\rRange 0% <-> 100%.,  default is 5%")]
        [DisplayName("HT Ratio")]
        [TypeConverter(typeof(NumericUpDownTypeConverter))]
        [Editor(typeof(NumericUpDownTypeEditor), typeof(UITypeEditor)), MinMax(0f, 100f, 5f, 2)]
        public virtual float MMHTRatio
        {
            get; set;
        }
        [Category("04.Measure"), DefaultValue(0)]
        [Description("Whole Ratio.\rRange 0 <-> 1000.,  default is 5")]
        [DisplayName("WH Ratio")]
        [TypeConverter(typeof(NumericUpDownTypeConverter))]
        [Editor(typeof(NumericUpDownTypeEditor), typeof(UITypeEditor)), MinMax(0f, 100f, 5f, 2)]
        public virtual float MMWholeRatio
        {
            get; set;
        }

        #endregion
        #region OCR Factor
        [Category("05.OCR or Barcode"), DefaultValue(OCRMethodEnum.NONE)]
        [Description("Using OCR or Barcode Method.")]
        [DisplayName("O.Method")]
        [TypeConverter(typeof(JzEnumConverter))]
        public virtual OCRMethodEnum OCRMethod
        {
            get; set;
        }
        [Category("05.OCR or Barcode"), DefaultValue("None")]
        [Description("OCR Parameter.")]
        [Browsable(true)]
        [TypeConverter(typeof(JzOCRStringConverter))]
        [DisplayName("OCR Para")]
        public virtual string OCRMappingMethod
        {
            get; set;
        }

        [Category("05.OCR or Barcode"), DefaultValue(0)]
        [Description("OCR MappingTextIndex.")]
        [Browsable(true)]
        [ReadOnly(false)]
        [TypeConverter(typeof(NumericUpDownTypeConverter))]
        [Editor(typeof(NumericUpDownTypeEditor), typeof(UITypeEditor)), MinMax(0, 1000)]
        [DisplayName("OCR Para")]
        public virtual int MappingTextIndex
        {
            get; set;
        }

        #endregion

        #region AOI Factor

        [Category("06.AOI"), DefaultValue(AOIMethodEnum.NONE)]
        [Description("Using AOI Method.")]
        [DisplayName("AOI Method")]
        [TypeConverter(typeof(JzEnumConverter))]
        public virtual AOIMethodEnum AOIMethod
        {
            get; set;
        }

        [Category("06.AOI"), DefaultValue(CheckDirtMethodEnum.NONE)]
        [Description("Check Dirt Method.")]
        [DisplayName("Check Dirt")]
        [TypeConverter(typeof(JzEnumConverter))]
        public virtual CheckDirtMethodEnum CheckDirtMethod
        {
            get; set;
        }

        [Category("06.AOI"), DefaultValue(UselessMethodEnum.NONE)]
        [Description("Check Color Method.")]
        [DisplayName("Check Color")]
        [TypeConverter(typeof(JzEnumConverter))]
        public virtual UselessMethodEnum CheckColorMethod
        {
            get; set;
        }
        [Category("06.AOI"), DefaultValue(false)]
        [Description("I am NG.")]
        [DisplayName("I'm NG")]
        public virtual bool IsNG
        {
            get; set;
        }
        [Category("06.AOI"), DefaultValue(20)]
        [Description("Dirt Ratio.\rRange 0% <-> 100%.,  default is 20(%)")]
        [DisplayName("D.Ratio")]
        [TypeConverter(typeof(NumericUpDownTypeConverter))]
        [Editor(typeof(NumericUpDownTypeEditor), typeof(UITypeEditor)), MinMax(0f, 100f, 0.1f, 3)]
        public virtual float DirtRatio
        {
            get; set;
        }
        [Category("06.AOI"), DefaultValue(10)]
        [Description("Dirt Area.\rRange 0 <-> 200.,  default is 10")]
        [DisplayName("D.Area")]
        [TypeConverter(typeof(NumericUpDownTypeConverter))]
        [Editor(typeof(NumericUpDownTypeEditor), typeof(UITypeEditor)), MinMax(0, 100)]
        public virtual int DirtArea
        {
            get; set;
        }
        [Category("06.AOI"), DefaultValue(40)]
        [Description("Color Ratio.\rRange 0% <-> 100%.,  default is 40(%)")]
        [DisplayName("C.Ratio")]
        [TypeConverter(typeof(NumericUpDownTypeConverter))]
        [Editor(typeof(NumericUpDownTypeEditor), typeof(UITypeEditor)), MinMax(0f, 100f, 0.1f, 2)]
        public virtual float ColorRatio
        {
            get; set;
        }
        [Category("06.AOI"), DefaultValue(0.02)]
        [Description("Total Color Ratio.\rRange 0% <-> 100%.,  default is 70(%)")]
        [DisplayName("TC.Ratio")]
        [TypeConverter(typeof(NumericUpDownTypeConverter))]
        [Editor(typeof(NumericUpDownTypeEditor), typeof(UITypeEditor)), MinMax(0f, 100f, 0.1f, 3)]
        public virtual float TotalColorRatio
        {
            get; set;
        }

        [Category("06.AOI")]
        [Description("Left Top Corner Check Setting")]
        [DisplayName("LTCorner")]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public virtual CornerClass LTCorner
        {
            get; set;
        }
        [Category("06.AOI")]
        [Description("Right Top Corner Check Setting")]
        [DisplayName("RTCorner")]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public virtual CornerClass RTCorner
        {
            get; set;
        }
        [Category("06.AOI")]
        [Description("Left Buttom Corner Check Setting")]
        [DisplayName("LBCorner")]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public virtual CornerClass LBCorner
        {
            get; set;
        }
        [Category("06.AOI")]
        [Description("Right Bottom Corner Check Setting")]
        [DisplayName("RBCorner")]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public virtual CornerClass RBCorner
        {
            get; set;
        }

        [Category("06.AOI")]
        [Description("XDir Width Check Setting")]
        [DisplayName("XDIR Measure")]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public virtual WHClass XDirWidtHeight
        {
            get; set;
        }

        [Category("06.AOI")]
        [Description("YDir Width Check Setting")]
        [DisplayName("YDIR Measure")]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public virtual WHClass YDirWidtHeight
        {
            get; set;
        }


        #endregion
        #region HEIGHT Factor

        [Category("07.Height"), DefaultValue(HeightMethodEnum.NONE)]
        [Description("Using HEIGHT Method.")]
        [DisplayName("Height Method")]
        [TypeConverter(typeof(JzEnumConverter))]
        public virtual HeightMethodEnum HeightMethod
        {
            get; set;
        }

        #endregion
        #region GAP Factor


        [Category("08.Gap"), DefaultValue(GapMethodEnum.NONE)]
        [Description("Using Gap Method.")]
        [DisplayName("Gap Method")]
        [TypeConverter(typeof(JzEnumConverter))]
        public virtual GapMethodEnum GapMethod
        {
            get; set;
        }

        [Category("08.Gap"), DefaultValue(5)]
        [Description("充许的最大偏移角度，单位：度。")]
        [DisplayName("1.偏移角度")]
        [TypeConverter(typeof(NumericUpDownTypeConverter))]
        [Editor(typeof(NumericUpDownTypeEditor), typeof(UITypeEditor)), MinMax(0f, 180f, 0.1f, 3)]
        public virtual float OffsetAngle
        {
            get; set;
        }

        [Category("08.Gap"), DefaultValue(30)]
        [Description("距离横边最大的距离 单位毫米。")]
        [DisplayName("2.最大距离")]
        [TypeConverter(typeof(NumericUpDownTypeConverter))]
        [Editor(typeof(NumericUpDownTypeEditor), typeof(UITypeEditor)), MinMax(0f, 1000f, 0.1f, 3)]
        public virtual float OffsetToUPMax
        {
            get; set;
        }
        [Category("08.Gap"), DefaultValue(20)]
        [Description("距离横边最小的距离 单位毫米。")]
        [DisplayName("3.最小距离")]
        [TypeConverter(typeof(NumericUpDownTypeConverter))]
        [Editor(typeof(NumericUpDownTypeEditor), typeof(UITypeEditor)), MinMax(0f, 1000f, 0.1f, 3)]
        public virtual float OffsetToUPMin
        {
            get; set;
        }

        [Category("08.Gap"), DefaultValue(10)]
        [Description("镭雕距离左边和右边最大差异 单位：毫米。")]
        [DisplayName("4.左右差异")]
        [TypeConverter(typeof(NumericUpDownTypeConverter))]
        [Editor(typeof(NumericUpDownTypeEditor), typeof(UITypeEditor)), MinMax(0f, 100f, 0.1f, 3)]
        public virtual float OffsetToLeftRight
        {
            get; set;
        }


        /// <summary>
        /// A位置最大值
        /// </summary>
        [Category("08.Gap"), DefaultValue(20)]
        [Description("A位置最大值 单位毫米。")]
        [DisplayName("5.A最大值")]
        [TypeConverter(typeof(NumericUpDownTypeConverter))]
        [Editor(typeof(NumericUpDownTypeEditor), typeof(UITypeEditor)), MinMax(0f, 1000f, 0.1f, 3)]
        public virtual float A_Max
        {
            get; set;
        }
        /// <summary>
        /// A位置最小值
        /// </summary>
        [Category("08.Gap"), DefaultValue(20)]
        [Description("A位置最小值 单位毫米。")]
        [DisplayName("6.A最小值")]
        [TypeConverter(typeof(NumericUpDownTypeConverter))]
        [Editor(typeof(NumericUpDownTypeEditor), typeof(UITypeEditor)), MinMax(0f, 1000f, 0.1f, 3)]
        public virtual float A_Min
        {
            get; set;
        }
        /// <summary>
        /// B位置最大值
        /// </summary>
        [Category("08.Gap"), DefaultValue(20)]
        [Description("B位置最大值 单位毫米。")]
        [DisplayName("7.B最大值")]
        [TypeConverter(typeof(NumericUpDownTypeConverter))]
        [Editor(typeof(NumericUpDownTypeEditor), typeof(UITypeEditor)), MinMax(0f, 1000f, 0.1f, 3)]
        public virtual float B_Max
        {
            get; set;
        }
        /// <summary>
        /// B位置最小值
        /// </summary>
        [Category("08.Gap"), DefaultValue(20)]
        [Description("B位置最小值 单位毫米。")]
        [DisplayName("8.B最小值")]
        [TypeConverter(typeof(NumericUpDownTypeConverter))]
        [Editor(typeof(NumericUpDownTypeEditor), typeof(UITypeEditor)), MinMax(0f, 1000f, 0.1f, 3)]
        public virtual float B_Min
        {
            get; set;
        }
        /// <summary>
        /// C位置最大值
        /// </summary>
        [Category("08.Gap"), DefaultValue(20)]
        [Description("C位置最大值 单位毫米。")]
        [DisplayName("9.C最大值")]
        [TypeConverter(typeof(NumericUpDownTypeConverter))]
        [Editor(typeof(NumericUpDownTypeEditor), typeof(UITypeEditor)), MinMax(0f, 1000f, 0.1f, 3)]
        public virtual float C_Max
        {
            get; set;
        }
        /// <summary>
        /// C位置最小值
        /// </summary>
        [Category("08.Gap"), DefaultValue(20)]
        [Description("C位置最小值 单位毫米。")]
        [DisplayName("10.C最小值")]
        [TypeConverter(typeof(NumericUpDownTypeConverter))]
        [Editor(typeof(NumericUpDownTypeEditor), typeof(UITypeEditor)), MinMax(0f, 1000f, 0.1f, 3)]
        public virtual float C_Min
        {
            get; set;
        }
        /// <summary>
        /// D位置最大值
        /// </summary>
        [Category("08.Gap"), DefaultValue(20)]
        [Description("D位置最大值 单位毫米。")]
        [DisplayName("11.D最大值")]
        [TypeConverter(typeof(NumericUpDownTypeConverter))]
        [Editor(typeof(NumericUpDownTypeEditor), typeof(UITypeEditor)), MinMax(0f, 1000f, 0.1f, 3)]
        public virtual float D_Max
        {
            get; set;
        }
        /// <summary>
        /// D位置最小值
        /// </summary>
        [Category("08.Gap"), DefaultValue(20)]
        [Description("D位置最小值 单位毫米。")]
        [DisplayName("12.D最小值")]
        [TypeConverter(typeof(NumericUpDownTypeConverter))]
        [Editor(typeof(NumericUpDownTypeEditor), typeof(UITypeEditor)), MinMax(0f, 1000f, 0.1f, 3)]
        public virtual float D_Min
        {
            get; set;
        }
        /// <summary>
        /// E位置最大值
        /// </summary>
        [Category("08.Gap"), DefaultValue(20)]
        [Description("E位置最大值 单位毫米。")]
        [DisplayName("13.E最大值")]
        [TypeConverter(typeof(NumericUpDownTypeConverter))]
        [Editor(typeof(NumericUpDownTypeEditor), typeof(UITypeEditor)), MinMax(0f, 1000f, 0.1f, 3)]
        public virtual float E_Max
        {
            get; set;
        }
        /// <summary>
        /// E位置最小值
        /// </summary>
        [Category("08.Gap"), DefaultValue(20)]
        [Description("E位置最小值 单位毫米。")]
        [DisplayName("14.E最小值")]
        [TypeConverter(typeof(NumericUpDownTypeConverter))]
        [Editor(typeof(NumericUpDownTypeEditor), typeof(UITypeEditor)), MinMax(0f, 1000f, 0.1f, 3)]
        public virtual float E_Min
        {
            get; set;
        }
        /// <summary>
        /// F位置最大值
        /// </summary>
        [Category("08.Gap"), DefaultValue(20)]
        [Description("F位置最大值 单位毫米。")]
        [DisplayName("15.F最大值")]
        [TypeConverter(typeof(NumericUpDownTypeConverter))]
        [Editor(typeof(NumericUpDownTypeEditor), typeof(UITypeEditor)), MinMax(0f, 1000f, 0.1f, 3)]
        public virtual float F_Max
        {
            get; set;
        }
        /// <summary>
        /// F位置最小值
        /// </summary>
        [Category("08.Gap"), DefaultValue(20)]
        [Description("F位置最小值 单位毫米。")]
        [DisplayName("16.F最小值")]
        [TypeConverter(typeof(NumericUpDownTypeConverter))]
        [Editor(typeof(NumericUpDownTypeEditor), typeof(UITypeEditor)), MinMax(0f, 1000f, 0.1f, 3)]
        public virtual float F_Min
        {
            get; set;
        }
        /// <summary>
        /// G位置最大值
        /// </summary>
        [Category("08.Gap"), DefaultValue(20)]
        [Description("G位置最大值 单位毫米。")]
        [DisplayName("17.G最大值")]
        [TypeConverter(typeof(NumericUpDownTypeConverter))]
        [Editor(typeof(NumericUpDownTypeEditor), typeof(UITypeEditor)), MinMax(0f, 1000f, 0.1f, 3)]
        public virtual float G_Max
        {
            get; set;
        }
        /// <summary>
        /// G位置最小值
        /// </summary>
        [Category("08.Gap"), DefaultValue(20)]
        [Description("G位置最小值 单位毫米。")]
        [DisplayName("18.G最小值")]
        [TypeConverter(typeof(NumericUpDownTypeConverter))]
        [Editor(typeof(NumericUpDownTypeEditor), typeof(UITypeEditor)), MinMax(0f, 1000f, 0.1f, 3)]
        public virtual float G_Min
        {
            get; set;
        }
        /// <summary>
        /// H位置最大值
        /// </summary>
        [Category("08.Gap"), DefaultValue(20)]
        [Description("H位置最大值 单位毫米。")]
        [DisplayName("19.H最大值")]
        [TypeConverter(typeof(NumericUpDownTypeConverter))]
        [Editor(typeof(NumericUpDownTypeEditor), typeof(UITypeEditor)), MinMax(0f, 1000f, 0.1f, 3)]
        public virtual float H_Max
        {
            get; set;
        }
        /// <summary>
        /// H位置最小值
        /// </summary>
        [Category("08.Gap"), DefaultValue(20)]
        [Description("H位置最小值 单位毫米。")]
        [DisplayName("20.H最小值")]
        [TypeConverter(typeof(NumericUpDownTypeConverter))]
        [Editor(typeof(NumericUpDownTypeEditor), typeof(UITypeEditor)), MinMax(0f, 1000f, 0.1f, 3)]
        public virtual float H_Min
        {
            get; set;
        }
        #endregion


        [Category("09.Stilts"), DefaultValue(STILTSMethodEnum.NONE)]
        [Description("是否检查高翘模式.")]
        [DisplayName("0.是否检查高翘")]
        [TypeConverter(typeof(JzEnumConverter))]
        public virtual STILTSMethodEnum STILTSMethod
        {
            get; set;
        }

        [Category("09.Stilts"), DefaultValue(5)]
        [Description("充许的最大长度，单位：像素。")]
        [DisplayName("1.高翘的长度")]
        [TypeConverter(typeof(NumericUpDownTypeConverter))]
        [Editor(typeof(NumericUpDownTypeEditor), typeof(UITypeEditor)), MinMax(0f, 180f, 0.1f, 3)]
        public virtual int StiltsOffSet
        {
            get; set;
        }

        [Category("09.Stilts"), DefaultValue(70)]
        [Description("阴影的灰度值，范围0-255。")]
        [DisplayName("2.阴影灰度")]
        [TypeConverter(typeof(NumericUpDownTypeConverter))]
        [Editor(typeof(NumericUpDownTypeEditor), typeof(UITypeEditor)), MinMax(1, 255f, 1, 0)]
        public virtual int StiltsGrayValue
        {
            get; set;
        }
        [Category("09.Stilts"), DefaultValue(70)]
        [Description("非阴影区最小灰度值，范围0-255。")]
        [DisplayName("3.非阴影最小灰度")]
        [TypeConverter(typeof(NumericUpDownTypeConverter))]
        [Editor(typeof(NumericUpDownTypeEditor), typeof(UITypeEditor)), MinMax(1, 255f, 1, 0)]
        public virtual int StiltsNOGrayValue
        {
            get; set;
        }

        #region PADCHECK

        [Category("10.PADCHECK"), DefaultValue(PADMethodEnum.NONE)]
        [Description("是否检查PAD溢胶模式")]
        [DisplayName("0.检查溢胶模式")]
        [TypeConverter(typeof(JzEnumConverter))]
        public virtual PADMethodEnum PADMethod
        {
            get; set;
        }

        [Category("10.PADCHECK"), DefaultValue(0.1)]
        [Description("与模板的长度比较的比例 大于此值NG 反之OK 无胶的长度比例")]
        [DisplayName("1.长度比例")]
        [TypeConverter(typeof(NumericUpDownTypeConverter))]
        [Editor(typeof(NumericUpDownTypeEditor), typeof(UITypeEditor)), MinMax(0f, 100f, 1, 2)]
        public virtual double PADOWidthRatio
        {
            get; set;
        }
        [Category("10.PADCHECK"), DefaultValue(0.1)]
        [Description("与模板的宽度比较的比例 大于此值NG 反之OK 无胶的宽度比例")]
        [DisplayName("2.宽度比例")]
        [TypeConverter(typeof(NumericUpDownTypeConverter))]
        [Editor(typeof(NumericUpDownTypeEditor), typeof(UITypeEditor)), MinMax(0f, 100f, 1, 2)]
        public virtual double PADOHeightRatio
        {
            get; set;
        }
        [Category("10.PADCHECK"), DefaultValue(0.1)]
        [Description("与模板的面积比较的比例 大于此值NG 反之OK")]
        [DisplayName("3.面积比例")]
        [TypeConverter(typeof(NumericUpDownTypeConverter))]
        [Editor(typeof(NumericUpDownTypeEditor), typeof(UITypeEditor)), MinMax(0f, 100f, 1, 2)]
        public virtual double PADOAreaRatio
        {
            get; set;
        }

        [Category("10.PADCHECK"), DefaultValue(PADThresholdEnum.Threshold)]
        [Description("检查PAD图像模式")]
        [DisplayName("4.图像模式")]
        [TypeConverter(typeof(JzEnumConverter))]
        public virtual PADThresholdEnum PADThresholdMode
        {
            get; set;
        }
        [Category("10.PADCHECK"), DefaultValue(true)]
        [Description("")]
        [DisplayName("4A.找芯片底色白色")]
        //[TypeConverter(typeof(NumericUpDownTypeConverter))]
        //[Editor(typeof(NumericUpDownTypeEditor), typeof(UITypeEditor)), MinMax(0f, 1000000f, 1, 2)]
        public virtual bool ChipFindWhite
        {
            get; set;
        }
        [Category("10.PADCHECK"), DefaultValue(128)]
        [Description("即 寻找中心位置的灰阶阈值")]
        [DisplayName("4.中心灰阶值")]
        [TypeConverter(typeof(NumericUpDownTypeConverter))]
        [Editor(typeof(NumericUpDownTypeEditor), typeof(UITypeEditor)), MinMax(0f, 255, 1, 0)]
        public virtual int PADGrayThreshold
        {
            get; set;
        }
        [Category("10.PADCHECK"), DefaultValue(128)]
        [Description("即 寻找中心位置中溢胶的灰阶阈值")]
        [DisplayName("5.中心溢胶灰阶值")]
        [TypeConverter(typeof(NumericUpDownTypeConverter))]
        [Editor(typeof(NumericUpDownTypeEditor), typeof(UITypeEditor)), MinMax(0f, 255, 1, 0)]
        public virtual int PADBlobGrayThreshold
        {
            get; set;
        }
        [Category("10.PADCHECK"), DefaultValue(true)]
        [Description("")]
        [DisplayName("5B.找银胶底色白色")]
        //[TypeConverter(typeof(NumericUpDownTypeConverter))]
        //[Editor(typeof(NumericUpDownTypeEditor), typeof(UITypeEditor)), MinMax(0f, 1000000f, 1, 2)]
        public virtual bool GLEFindWhite
        {
            get; set;
        }
        [Category("10.PADCHECK"), DefaultValue(100)]
        [Description("即 寻找中心位置中银胶的灰阶阈值")]
        [DisplayName("5A.银胶灰阶值")]
        [TypeConverter(typeof(NumericUpDownTypeConverter))]
        [Editor(typeof(NumericUpDownTypeEditor), typeof(UITypeEditor)), MinMax(0f, 255, 1, 0)]
        public virtual int PADChipInBlobGrayThreshold
        {
            get; set;
        }
        [Category("10.PADCHECK"), DefaultValue(15)]
        [Description("溢胶的面积大小 大于此值NG 反之OK (單位 mm)")]
        [DisplayName("6.检查溢胶面积")]
        [TypeConverter(typeof(NumericUpDownTypeConverter))]
        [Editor(typeof(NumericUpDownTypeEditor), typeof(UITypeEditor)), MinMax(0f, 1000000f, 1, 2)]
        public virtual double PADCheckDArea
        {
            get; set;
        }
        [Category("10.PADCHECK"), DefaultValue(15)]
        [Description("溢胶的长度 大于此值NG 反之OK (單位 mm)")]
        [DisplayName("7.检查溢胶长度")]
        [TypeConverter(typeof(NumericUpDownTypeConverter))]
        [Editor(typeof(NumericUpDownTypeEditor), typeof(UITypeEditor)), MinMax(0f, 1000000f, 1, 2)]
        public virtual double PADCheckDWidth
        {
            get; set;
        }
        [Category("10.PADCHECK"), DefaultValue(15)]
        [Description("溢胶的宽度 大于此值NG 反之OK (單位 mm)")]
        [DisplayName("7.检查溢胶宽度")]
        [TypeConverter(typeof(NumericUpDownTypeConverter))]
        [Editor(typeof(NumericUpDownTypeEditor), typeof(UITypeEditor)), MinMax(0f, 1000000f, 1, 2)]
        public virtual double PADCheckDHeight
        {
            get; set;
        }

        [Category("11.PADCHECK"), DefaultValue(5)]
        [Description("")]
        [DisplayName("8.X方向 (單位 mm)")]
        [TypeConverter(typeof(NumericUpDownTypeConverter))]
        [Editor(typeof(NumericUpDownTypeEditor), typeof(UITypeEditor)), MinMax(0f, 1000000f, 1, 2)]
        public virtual double PADExtendX
        {
            get; set;
        }
        [Category("12.PADCHECK"), DefaultValue(5)]
        [Description("")]
        [DisplayName("9.Y方向 (單位 mm)")]
        [TypeConverter(typeof(NumericUpDownTypeConverter))]
        [Editor(typeof(NumericUpDownTypeEditor), typeof(UITypeEditor)), MinMax(0f, 1000000f, 1, 2)]
        public virtual double PADExtendY
        {
            get; set;
        }
        [Category("11.PADCHECK"), DefaultValue(5)]
        [Description("")]
        [DisplayName("D1.胶水X方向 (單位 mm)")]
        [TypeConverter(typeof(NumericUpDownTypeConverter))]
        [Editor(typeof(NumericUpDownTypeEditor), typeof(UITypeEditor)), MinMax(0f, 1000000f, 1, 2)]
        public virtual double CalExtendX
        {
            get; set;
        }
        [Category("12.PADCHECK"), DefaultValue(5)]
        [Description("")]
        [DisplayName("D2.胶水Y方向 (單位 mm)")]
        [TypeConverter(typeof(NumericUpDownTypeConverter))]
        [Editor(typeof(NumericUpDownTypeEditor), typeof(UITypeEditor)), MinMax(0f, 1000000f, 1, 2)]
        public virtual double CalExtendY
        {
            get; set;
        }
        [Category("11.PADCHECK"), DefaultValue(5)]
        [Description("")]
        [DisplayName("D3.黑边X (單位 mil)")]
        [TypeConverter(typeof(NumericUpDownTypeConverter))]
        [Editor(typeof(NumericUpDownTypeEditor), typeof(UITypeEditor)), MinMax(-1000f, 1000000f, 1, 2)]
        public virtual double BlackCalExtendX
        {
            get; set;
        }
        [Category("12.PADCHECK"), DefaultValue(5)]
        [Description("")]
        [DisplayName("D4.黑边Y (單位 mil)")]
        [TypeConverter(typeof(NumericUpDownTypeConverter))]
        [Editor(typeof(NumericUpDownTypeEditor), typeof(UITypeEditor)), MinMax(-1000f, 1000000f, 1, 2)]
        public virtual double BlackCalExtendY
        {
            get; set;
        }
        [Category("11.PADCHECK"), DefaultValue(5)]
        [Description("")]
        [DisplayName("D5.黑边X偏移 (單位 mil)")]
        [TypeConverter(typeof(NumericUpDownTypeConverter))]
        [Editor(typeof(NumericUpDownTypeEditor), typeof(UITypeEditor)), MinMax(-1000f, 1000000f, 1, 2)]
        public virtual double BlackOffsetX
        {
            get; set;
        }
        [Category("12.PADCHECK"), DefaultValue(5)]
        [Description("")]
        [DisplayName("D6.黑边Y偏移 (單位 mil)")]
        [TypeConverter(typeof(NumericUpDownTypeConverter))]
        [Editor(typeof(NumericUpDownTypeEditor), typeof(UITypeEditor)), MinMax(-1000f, 1000000f, 1, 2)]
        public virtual double BlackOffsetY
        {
            get; set;
        }

        [Category("12.PADCHECK"), DefaultValue(35)]
        [Description("")]
        [DisplayName("D7.字体大小 (單位 pix)")]
        [TypeConverter(typeof(NumericUpDownTypeConverter))]
        [Editor(typeof(NumericUpDownTypeEditor), typeof(UITypeEditor)), MinMax(-1000f, 1000000f, 1, 2)]
        public virtual int FontSize
        {
            get; set;
        }
        [Category("12.PADCHECK"), DefaultValue(5)]
        [Description("")]
        [DisplayName("D8.线宽 (單位 pix)")]
        [TypeConverter(typeof(NumericUpDownTypeConverter))]
        [Editor(typeof(NumericUpDownTypeEditor), typeof(UITypeEditor)), MinMax(-1000f, 1000000f, 1, 2)]
        public virtual int LineWidth
        {
            get; set;
        }
        [Category("10.PADCHECK"), DefaultValue(ChipSlotDir.NONE)]
        [Description("即 用来测试薄膜胶的宽度")]
        [DisplayName("D9.标记槽的方向")]
        [TypeConverter(typeof(JzEnumConverter))]
        public virtual ChipSlotDir GlueChipSlotDir
        {
            get; set;
        }

        [Category("11.PADCHECK"), DefaultValue(5)]
        [Description("")]
        [DisplayName("spec.薄膜胶最大值 (單位 mm)")]
        [TypeConverter(typeof(NumericUpDownTypeConverter))]
        [Editor(typeof(NumericUpDownTypeEditor), typeof(UITypeEditor)), MinMax(0f, 1000000f, 1, 2)]
        public virtual double GlueMax
        {
            get; set;
        }
        [Category("11.PADCHECK"), DefaultValue(5)]
        [Description("")]
        [DisplayName("spec.薄膜胶最小值 (單位 mm)")]
        [TypeConverter(typeof(NumericUpDownTypeConverter))]
        [Editor(typeof(NumericUpDownTypeEditor), typeof(UITypeEditor)), MinMax(0f, 1000000f, 1, 2)]
        public virtual double GlueMin
        {
            get; set;
        }
        [Category("11.PADCHECK"), DefaultValue(false)]
        [Description("")]
        [DisplayName("A1a.是否检查胶水宽度")]
        //[TypeConverter(typeof(NumericUpDownTypeConverter))]
        //[Editor(typeof(NumericUpDownTypeEditor), typeof(UITypeEditor)), MinMax(0f, 1000000f, 1, 2)]
        public virtual bool GlueCheck
        {
            get; set;
        }
        [Category("11.PADCHECK"), DefaultValue(true)]
        [Description("")]
        [DisplayName("A1b.芯片水平方向")]
        //[TypeConverter(typeof(NumericUpDownTypeConverter))]
        //[Editor(typeof(NumericUpDownTypeEditor), typeof(UITypeEditor)), MinMax(0f, 1000000f, 1, 2)]
        public virtual bool ChipDirLevel
        {
            get; set;
        }
        [Category("11.PADCHECK"), DefaultValue(true)]
        [Description("")]
        [DisplayName("A1c.芯片接触锡球检测")]
        //[TypeConverter(typeof(NumericUpDownTypeConverter))]
        //[Editor(typeof(NumericUpDownTypeEditor), typeof(UITypeEditor)), MinMax(0f, 1000000f, 1, 2)]
        public virtual bool ChipGleCheck
        {
            get; set;
        }
        [Category("11.PADCHECK"), DefaultValue(0)]
        [Description("小于设定的值则为四周无胶PASS，反之NG")]
        [DisplayName("A1d.检测四周无胶的值")]
        [TypeConverter(typeof(NumericUpDownTypeConverter))]
        [Editor(typeof(NumericUpDownTypeEditor), typeof(UITypeEditor)), MinMax(0, 255)]
        public virtual int FourSideNoGluePassValue
        {
            get; set;
        }
        [Category("10.PADCHECK"), DefaultValue(""), ReadOnly(true)]
        //[Description("大芯片与小芯片处理图像模式")]
        [DisplayName("A4a.检测无芯片方法")]
        [TypeConverter(typeof(JzEnumConverter))]
        public virtual ChipNoHave ChipNoHaveMode
        {
            get; set;
        }
        [Category("10.PADCHECK"), DefaultValue("")]
        [Description("")]
        [DisplayName("A4a.检测无芯片方法参数")]
        [Editor(typeof(GetNoHaveModePropertyEditor), typeof(UITypeEditor))]
        //[TypeConverter(typeof(NumericUpDownTypeConverter))]
        //[Editor(typeof(NumericUpDownTypeEditor), typeof(UITypeEditor)), MinMax(0f, 1f, 0.1f, 2)]
        public virtual string ChipNoHaveModeOpString
        {
            get; set;
        }
        [Category("10.PADCHECK"), DefaultValue(ChipNoGlueMethod.NONE)]
        //[Description("大芯片与小芯片处理图像模式")]
        [DisplayName("A4b.检测无胶方法")]
        [TypeConverter(typeof(JzEnumConverter))]
        public virtual ChipNoGlueMethod ChipNoGlueMode
        {
            get; set;
        }
        [Category("11.PADCHECK"), DefaultValue(0.75)]
        [Description("")]
        [DisplayName("A4c.检测无胶容许值")]
        [TypeConverter(typeof(NumericUpDownTypeConverter))]
        [Editor(typeof(NumericUpDownTypeEditor), typeof(UITypeEditor)), MinMax(0f, 1f, 0.1f, 2)]
        public virtual double NoGlueThresholdValue
        {
            get; set;
        }
        [Category("10.PADCHECK"), DefaultValue(PADCalModeEnum.BlacktoBlack)]
        [Description("计算宽度模式 有电阻模式即周围有电阻抓点到最外边 反之周围无电阻且抓点停止于最大尺寸")]
        [DisplayName("B.计算宽度模式")]
        [TypeConverter(typeof(JzEnumConverter))]
        public virtual PADCalModeEnum PADCalMode
        {
            get; set;
        }
        [Category("10.PADCHECK"), DefaultValue(PADCalModeEnum.BlacktoBlack)]
        [Description("大芯片与小芯片处理图像模式")]
        [DisplayName("C.计算宽度图像模式")]
        [TypeConverter(typeof(JzEnumConverter))]
        public virtual PADChipSize PADChipSizeMode
        {
            get; set;
        }
        [Category("10.PADCHECK"), DefaultValue(0.33)]
        [Description("调整图像的边界的容许值百分比值")]
        [DisplayName("C0.通用模式容许值")]
        [TypeConverter(typeof(NumericUpDownTypeConverter))]
        [Editor(typeof(NumericUpDownTypeEditor), typeof(UITypeEditor)), MinMax(0f, 5f, 0.1f, 2)]
        public virtual double BloodFillValueRatio
        {
            get; set;
        }
        [Category("10.PADCHECK"), DefaultValue(""), ReadOnly(true)]
        [Description("")]
        [DisplayName("C1.通用模式模型")]
        //[TypeConverter(typeof(NumericUpDownTypeConverter))]
        //[Editor(typeof(NumericUpDownTypeEditor), typeof(UITypeEditor)), MinMax(0f, 1f, 0.1f, 2)]
        public virtual PadInspectMethodEnum PadInspectMethod
        {
            get; set;
        }
        [Category("10.PADCHECK"), DefaultValue("")]
        [Description("")]
        [DisplayName("C1.通用模式模型参数")]
        [Editor(typeof(GetPADDataPropertyEditor), typeof(UITypeEditor))]
        //[TypeConverter(typeof(NumericUpDownTypeConverter))]
        //[Editor(typeof(NumericUpDownTypeEditor), typeof(UITypeEditor)), MinMax(0f, 1f, 0.1f, 2)]
        public virtual string PADINSPECTOPString
        {
            get; set;
        }
        [Category("10.PADCHECK"), DefaultValue(AICategory.Baseline)]
        [Description("在AI模式下选择不同模型以适应芯片的大小")]
        [DisplayName("C1.AI模型")]
        [TypeConverter(typeof(JzEnumConverter))]
        public virtual AICategory PADAICategory
        {
            get; set;
        }

        [Category("10.PADCHECK"), DefaultValue("")]
        [Description("")]
        [DisplayName("E1.扩展参数")]
        [Editor(typeof(GetExtendPropertyEditor), typeof(UITypeEditor))]
        //[TypeConverter(typeof(NumericUpDownTypeConverter))]
        //[Editor(typeof(NumericUpDownTypeEditor), typeof(UITypeEditor)), MinMax(0f, 1f, 0.1f, 2)]
        public virtual string PADExtendOPString
        {
            get; set;
        }


        [Category("11.PADCHECK"), DefaultValue(0.6)]
        [Description("")]
        [DisplayName("spec.上最大值 (單位 mm)")]
        [TypeConverter(typeof(NumericUpDownTypeConverter))]
        [Editor(typeof(NumericUpDownTypeEditor), typeof(UITypeEditor)), MinMax(0f, 1000000f, 1, 2)]
        public virtual double GlueMaxTop
        {
            get; set;
        }
        [Category("11.PADCHECK"), DefaultValue(0.1)]
        [Description("")]
        [DisplayName("spec.上最小值 (單位 mm)")]
        [TypeConverter(typeof(NumericUpDownTypeConverter))]
        [Editor(typeof(NumericUpDownTypeEditor), typeof(UITypeEditor)), MinMax(0f, 1000000f, 1, 2)]
        public virtual double GlueMinTop
        {
            get; set;
        }

        [Category("11.PADCHECK"), DefaultValue(0.6)]
        [Description("")]
        [DisplayName("spec.下最大值 (單位 mm)")]
        [TypeConverter(typeof(NumericUpDownTypeConverter))]
        [Editor(typeof(NumericUpDownTypeEditor), typeof(UITypeEditor)), MinMax(0f, 1000000f, 1, 2)]
        public virtual double GlueMaxBottom
        {
            get; set;
        }
        [Category("11.PADCHECK"), DefaultValue(0.1)]
        [Description("")]
        [DisplayName("spec.下最小值 (單位 mm)")]
        [TypeConverter(typeof(NumericUpDownTypeConverter))]
        [Editor(typeof(NumericUpDownTypeEditor), typeof(UITypeEditor)), MinMax(0f, 1000000f, 1, 2)]
        public virtual double GlueMinBottom
        {
            get; set;
        }

        [Category("11.PADCHECK"), DefaultValue(1.2)]
        [Description("")]
        [DisplayName("spec.左最大值 (單位 mm)")]
        [TypeConverter(typeof(NumericUpDownTypeConverter))]
        [Editor(typeof(NumericUpDownTypeEditor), typeof(UITypeEditor)), MinMax(0f, 1000000f, 1, 2)]
        public virtual double GlueMaxLeft
        {
            get; set;
        }
        [Category("11.PADCHECK"), DefaultValue(0.1)]
        [Description("")]
        [DisplayName("spec.左最小值 (單位 mm)")]
        [TypeConverter(typeof(NumericUpDownTypeConverter))]
        [Editor(typeof(NumericUpDownTypeEditor), typeof(UITypeEditor)), MinMax(0f, 1000000f, 1, 2)]
        public virtual double GlueMinLeft
        {
            get; set;
        }

        [Category("11.PADCHECK"), DefaultValue(0.9)]
        [Description("")]
        [DisplayName("spec.右最大值 (單位 mm)")]
        [TypeConverter(typeof(NumericUpDownTypeConverter))]
        [Editor(typeof(NumericUpDownTypeEditor), typeof(UITypeEditor)), MinMax(0f, 1000000f, 1, 2)]
        public virtual double GlueMaxRight
        {
            get; set;
        }
        [Category("11.PADCHECK"), DefaultValue(0.1)]
        [Description("")]
        [DisplayName("spec.右最小值 (單位 mm)")]
        [TypeConverter(typeof(NumericUpDownTypeConverter))]
        [Editor(typeof(NumericUpDownTypeEditor), typeof(UITypeEditor)), MinMax(0f, 1000000f, 1, 2)]
        public virtual double GlueMinRight
        {
            get; set;
        }


        [Category("11.PADCHECK"), DefaultValue(0.1)]
        [Description("")]
        [DisplayName("specA.01.银胶长度上限 (單位 mm)")]
        [TypeConverter(typeof(NumericUpDownTypeConverter))]
        [Editor(typeof(NumericUpDownTypeEditor), typeof(UITypeEditor)), MinMax(0f, 1000000f, 1, 2)]
        public virtual double GleWidthUpper
        {
            get; set;
        }
        [Category("11.PADCHECK"), DefaultValue(0.1)]
        [Description("")]
        [DisplayName("specA.02.银胶长度下限 (單位 mm)")]
        [TypeConverter(typeof(NumericUpDownTypeConverter))]
        [Editor(typeof(NumericUpDownTypeEditor), typeof(UITypeEditor)), MinMax(0f, 1000000f, 1, 2)]
        public virtual double GleWidthLower
        {
            get; set;
        }

        [Category("11.PADCHECK"), DefaultValue(0.1)]
        [Description("")]
        [DisplayName("specA.03.银胶宽度上限 (單位 mm)")]
        [TypeConverter(typeof(NumericUpDownTypeConverter))]
        [Editor(typeof(NumericUpDownTypeEditor), typeof(UITypeEditor)), MinMax(0f, 1000000f, 1, 2)]
        public virtual double GleHeightUpper
        {
            get; set;
        }
        [Category("11.PADCHECK"), DefaultValue(0.1)]
        [Description("")]
        [DisplayName("specA.04.银胶宽度下限 (單位 mm)")]
        [TypeConverter(typeof(NumericUpDownTypeConverter))]
        [Editor(typeof(NumericUpDownTypeEditor), typeof(UITypeEditor)), MinMax(0f, 1000000f, 1, 2)]
        public virtual double GleHeightLower
        {
            get; set;
        }

        [Category("11.PADCHECK"), DefaultValue(0.1)]
        [Description("")]
        [DisplayName("specA.05.银胶面积上限 (單位 mm)")]
        [TypeConverter(typeof(NumericUpDownTypeConverter))]
        [Editor(typeof(NumericUpDownTypeEditor), typeof(UITypeEditor)), MinMax(0f, 1000000f, 1, 2)]
        public virtual double GleAreaUpper
        {
            get; set;
        }
        [Category("11.PADCHECK"), DefaultValue(0.1)]
        [Description("")]
        [DisplayName("specA.06.银胶面积下限 (單位 mm)")]
        [TypeConverter(typeof(NumericUpDownTypeConverter))]
        [Editor(typeof(NumericUpDownTypeEditor), typeof(UITypeEditor)), MinMax(0f, 1000000f, 1, 2)]
        public virtual double GleAreaLower
        {
            get; set;
        }

        [Category("11.PADCHECK"), DefaultValue(0.1)]
        [Description("")]
        [DisplayName("specA.07.银胶上下距离偏移量 (單位 mm)")]
        [TypeConverter(typeof(NumericUpDownTypeConverter))]
        [Editor(typeof(NumericUpDownTypeEditor), typeof(UITypeEditor)), MinMax(0f, 1000000f, 1, 2)]
        public virtual double GlueTopBottomOffset
        {
            get; set;
        }
        [Category("11.PADCHECK"), DefaultValue(0.1)]
        [Description("")]
        [DisplayName("specA.08.银胶左右距离偏移量 (單位 mm)")]
        [TypeConverter(typeof(NumericUpDownTypeConverter))]
        [Editor(typeof(NumericUpDownTypeEditor), typeof(UITypeEditor)), MinMax(0f, 1000000f, 1, 2)]
        public virtual double GlueLeftRightOffset
        {
            get; set;
        }

        #endregion


        public virtual void ConstructProperty(VersionEnum ver, OptionEnum opt)
        {
            #region NORMAL

            switch (opt)
            {
                case OptionEnum.MAIN_X6:

                    publicproperties.Add(new myProperty("Name", "01.Normal"));
                    publicproperties.Add(new myProperty("AliasName", "01.Normal"));
                    //publicproperties.Add(new myProperty("Brightness", "01.Normal"));
                    //publicproperties.Add(new myProperty("Contrast", "01.Normal"));
                    //publicproperties.Add(new myProperty("MaskMethod", "01.Normal"));
                    publicproperties.Add(new myProperty("ExtendX", "01.Normal"));
                    publicproperties.Add(new myProperty("ExtendY", "01.Normal"));
                    //publicproperties.Add(new myProperty("RelateASN", "01.Normal"));
                    //publicproperties.Add(new myProperty("RelateASNItem", "01.Normal"));


                    publicproperties.Add(new myProperty("AlignMethod", "02.Align"));
                    publicproperties.Add(new myProperty("AlignMode", "02.Align"));
                    publicproperties.Add(new myProperty("MTPSample", "02.Align"));
                    //publicproperties.Add(new myProperty("MTCannyAuto", "02.Align"));
                    //publicproperties.Add(new myProperty("MTCannyH", "02.Align"));
                    //publicproperties.Add(new myProperty("MTCannyL", "02.Align"));
                    publicproperties.Add(new myProperty("MTRotation", "02.Align"));
                    //publicproperties.Add(new myProperty("MTScaling", "02.Align"));
                    //publicproperties.Add(new myProperty("MTMaxOcc", "02.Align"));
                    publicproperties.Add(new myProperty("MTTolerance", "02.Align"));
                    //publicproperties.Add(new myProperty("MTIsSubPixel", "02.Align")); //No Use Now
                    publicproperties.Add(new myProperty("MTOffset", "02.Align"));
                    publicproperties.Add(new myProperty("MTResolution", "02.Align"));

                    publicproperties.Add(new myProperty("ABSAlignMethod", "02.Align"));
                    publicproperties.Add(new myProperty("ABSOffset", "02.Align"));

                    switch (Universal.FACTORYNAME)
                    {
                        case FactoryName.DAGUI:
                            break;
                        default:
                            publicproperties.Add(new myProperty("IsSeed", "01.Normal"));
                            break;
                    }


                    break;
                case OptionEnum.MAIN_SDM2:

                    publicproperties.Add(new myProperty("Name", "01.Normal"));
                    publicproperties.Add(new myProperty("AliasName", "01.Normal"));
                    publicproperties.Add(new myProperty("ExtendX", "01.Normal"));
                    publicproperties.Add(new myProperty("ExtendY", "01.Normal"));

                    publicproperties.Add(new myProperty("AlignMethod", "02.Align"));
                    publicproperties.Add(new myProperty("AlignMode", "02.Align"));
                    publicproperties.Add(new myProperty("MTPSample", "02.Align"));
                    publicproperties.Add(new myProperty("MTRotation", "02.Align"));
                    publicproperties.Add(new myProperty("MTTolerance", "02.Align"));

                    break;
                default:

                    publicproperties.Add(new myProperty("Name", "01.Normal"));
                    publicproperties.Add(new myProperty("AliasName", "01.Normal"));
                    publicproperties.Add(new myProperty("Brightness", "01.Normal"));
                    publicproperties.Add(new myProperty("Contrast", "01.Normal"));
                    publicproperties.Add(new myProperty("MaskMethod", "01.Normal"));
                    publicproperties.Add(new myProperty("ExtendX", "01.Normal"));
                    publicproperties.Add(new myProperty("ExtendY", "01.Normal"));
                    publicproperties.Add(new myProperty("RelateASN", "01.Normal"));
                    publicproperties.Add(new myProperty("RelateASNItem", "01.Normal"));
                    publicproperties.Add(new myProperty("IsSeed", "01.Normal"));

                    publicproperties.Add(new myProperty("AlignMethod", "02.Align"));
                    publicproperties.Add(new myProperty("AlignMode", "02.Align"));
                    publicproperties.Add(new myProperty("MTPSample", "02.Align"));
                    publicproperties.Add(new myProperty("MTCannyAuto", "02.Align"));
                    publicproperties.Add(new myProperty("MTCannyH", "02.Align"));
                    publicproperties.Add(new myProperty("MTCannyL", "02.Align"));
                    publicproperties.Add(new myProperty("MTRotation", "02.Align"));
                    publicproperties.Add(new myProperty("MTScaling", "02.Align"));
                    publicproperties.Add(new myProperty("MTMaxOcc", "02.Align"));
                    publicproperties.Add(new myProperty("MTTolerance", "02.Align"));
                    //publicproperties.Add(new myProperty("MTIsSubPixel", "02.Align")); //No Use Now
                    publicproperties.Add(new myProperty("MTOffset", "02.Align"));
                    publicproperties.Add(new myProperty("MTResolution", "02.Align"));

                    publicproperties.Add(new myProperty("ABSAlignMethod", "02.Align"));
                    publicproperties.Add(new myProperty("ABSOffset", "02.Align"));

                    break;
            }



            #endregion

            switch (ver)
            {
                case VersionEnum.ALLINONE:

                    switch (opt)
                    {
                        case OptionEnum.MAIN_SDM3:

                            #region 锡球检测

                            publicproperties.Add(new myProperty("InspectionMethod", "03.Inspection"));
                            publicproperties.Add(new myProperty("Inspection_A_B_Method", "03.Inspection"));
                            publicproperties.Add(new myProperty("IBCount", "03.Inspection"));
                            publicproperties.Add(new myProperty("IBArea", "03.Inspection"));
                            publicproperties.Add(new myProperty("IBTolerance", "03.Inspection"));

                            publicproperties.Add(new myProperty("MeasureMethod", "04.Measure"));
                            publicproperties.Add(new myProperty("MMOPString", "04.Measure"));

                            publicproperties.Add(new myProperty("OCRMethod", "05.OCR or Barcode", "字符或条码检测"));
                            publicproperties.Add(new myProperty("OCRMappingMethod", "05.OCR or Barcode", "字符或条码检测"));

                            publicproperties.Add(new myProperty("AOIMethod", "06.AOI"));
                            publicproperties.Add(new myProperty("CheckDirtMethod", "06.AOI"));
                            publicproperties.Add(new myProperty("IsNG", "06.AOI"));
                            publicproperties.Add(new myProperty("DirtRatio", "06.AOI"));
                            publicproperties.Add(new myProperty("DirtArea", "06.AOI"));

                            publicproperties.Add(new myProperty("LTCorner", "06.AOI"));
                            publicproperties.Add(new myProperty("RTCorner", "06.AOI"));
                            publicproperties.Add(new myProperty("LBCorner", "06.AOI"));
                            publicproperties.Add(new myProperty("RBCorner", "06.AOI"));

                            publicproperties.Add(new myProperty("XDirWidtHeight", "06.AOI"));
                            publicproperties.Add(new myProperty("YDirWidtHeight", "06.AOI"));

                            publicproperties.Add(new myProperty("PADMethod", "10.PADCHECK"));
                            publicproperties.Add(new myProperty("PADOWidthRatio", "10.PADCHECK"));
                            publicproperties.Add(new myProperty("PADOHeightRatio", "10.PADCHECK"));
                            publicproperties.Add(new myProperty("PADOAreaRatio", "10.PADCHECK"));
                            publicproperties.Add(new myProperty("PADGrayThreshold", "10.PADCHECK"));
                            publicproperties.Add(new myProperty("PADBlobGrayThreshold", "10.PADCHECK"));
                            publicproperties.Add(new myProperty("PADCheckDArea", "10.PADCHECK"));
                            publicproperties.Add(new myProperty("PADCheckDWidth", "10.PADCHECK"));
                            publicproperties.Add(new myProperty("PADCheckDHeight", "10.PADCHECK"));

                            publicproperties.Add(new myProperty("PADExtendX", "10.PADCHECK"));
                            publicproperties.Add(new myProperty("PADExtendY", "10.PADCHECK"));

                            publicproperties.Add(new myProperty("GlueMax", "10.PADCHECK"));
                            publicproperties.Add(new myProperty("GlueMin", "10.PADCHECK"));
                            publicproperties.Add(new myProperty("GlueCheck", "10.PADCHECK"));

                            publicproperties.Add(new myProperty("PADThresholdMode", "10.PADCHECK"));
                            publicproperties.Add(new myProperty("NoGlueThresholdValue", "10.PADCHECK"));
                            publicproperties.Add(new myProperty("PADCalMode", "10.PADCHECK"));
                            publicproperties.Add(new myProperty("PADChipSizeMode", "10.PADCHECK"));

                            publicproperties.Add(new myProperty("CalExtendX", "10.PADCHECK"));
                            publicproperties.Add(new myProperty("CalExtendY", "10.PADCHECK"));

                            publicproperties.Add(new myProperty("BloodFillValueRatio", "10.PADCHECK"));

                            publicproperties.Add(new myProperty("GlueMaxTop", "10.PADCHECK"));
                            publicproperties.Add(new myProperty("GlueMinTop", "10.PADCHECK"));
                            publicproperties.Add(new myProperty("GlueMaxBottom", "10.PADCHECK"));
                            publicproperties.Add(new myProperty("GlueMinBottom", "10.PADCHECK"));
                            publicproperties.Add(new myProperty("GlueMaxLeft", "10.PADCHECK"));
                            publicproperties.Add(new myProperty("GlueMinLeft", "10.PADCHECK"));
                            publicproperties.Add(new myProperty("GlueMaxRight", "10.PADCHECK"));
                            publicproperties.Add(new myProperty("GlueMinRight", "10.PADCHECK"));

                            publicproperties.Add(new myProperty("BlackCalExtendX", "10.PADCHECK"));
                            publicproperties.Add(new myProperty("BlackCalExtendY", "10.PADCHECK"));
                            publicproperties.Add(new myProperty("BlackOffsetX", "10.PADCHECK"));
                            publicproperties.Add(new myProperty("BlackOffsetY", "10.PADCHECK"));

                            #endregion

                            break;

                        case OptionEnum.MAIN_SDM2:

                            //publicproperties.Add(new myProperty("PADMethod", "10.PADCHECK"));
                            //#region 分层 选择测试项目则显示相应的参数

                            //switch(PADMethod)
                            //{
                            //    case PADMethodEnum.GLUECHECK:

                            //        publicproperties.Add(new myProperty("PADOWidthRatio", "10.PADCHECK"));
                            //        publicproperties.Add(new myProperty("PADOHeightRatio", "10.PADCHECK"));
                            //        publicproperties.Add(new myProperty("PADOAreaRatio", "10.PADCHECK"));
                            //        publicproperties.Add(new myProperty("PADThresholdMode", "10.PADCHECK"));
                            //        publicproperties.Add(new myProperty("PADGrayThreshold", "10.PADCHECK"));
                            //        publicproperties.Add(new myProperty("PADBlobGrayThreshold", "10.PADCHECK"));

                            //        publicproperties.Add(new myProperty("PADCheckDArea", "10.PADCHECK"));
                            //        publicproperties.Add(new myProperty("PADCheckDWidth", "10.PADCHECK"));
                            //        publicproperties.Add(new myProperty("PADCheckDHeight", "10.PADCHECK"));
                            //        publicproperties.Add(new myProperty("PADExtendX", "10.PADCHECK"));
                            //        publicproperties.Add(new myProperty("PADExtendY", "10.PADCHECK"));
                            //        publicproperties.Add(new myProperty("GlueCheck", "10.PADCHECK"));
                            //        publicproperties.Add(new myProperty("ChipDirlevel", "10.PADCHECK"));


                            //        break;
                            //    default:
                            //        break;
                            //}

                            //#endregion

                            //break;
                        case OptionEnum.MAIN_SDM1:
                        case OptionEnum.MAIN_SD:

                            //publicproperties.Add(new myProperty("MeasureMethod", "04.Measure"));
                            //publicproperties.Add(new myProperty("MMOPString", "04.Measure"));

                            #region OCR

                            //publicproperties.Add(new myProperty("InspectionMethod", "03.Inspection"));
                            //publicproperties.Add(new myProperty("Inspection_A_B_Method", "03.Inspection"));
                            //publicproperties.Add(new myProperty("IBCount", "03.Inspection"));
                            //publicproperties.Add(new myProperty("IBArea", "03.Inspection"));
                            //publicproperties.Add(new myProperty("IBTolerance", "03.Inspection"));

                            //publicproperties.Add(new myProperty("MeasureMethod", "04.Measure"));
                            ////publicproperties.Add(new myProperty("MMTolerance", "04.Measure"));
                            //publicproperties.Add(new myProperty("MMOPString", "04.Measure"));
                            ////publicproperties.Add(new myProperty("MMMaxGap", "04.Measure"));
                            ////publicproperties.Add(new myProperty("MMMinGap", "04.Measure"));
                            ////publicproperties.Add(new myProperty("MMPixelGap", "04.Measure"));
                            ////publicproperties.Add(new myProperty("MMHTRatio", "04.Measure"));
                            ////publicproperties.Add(new myProperty("MMWholeRatio", "04.Measure"));

                            publicproperties.Add(new myProperty("OCRMethod", "05.OCR or Barcode", "字符或条码检测"));
                            //publicproperties.Add(new myProperty("OCRMappingMethod", "05.OCR or Barcode", "字符或条码检测"));

                            //publicproperties.Add(new myProperty("AOIMethod", "06.AOI"));
                            //publicproperties.Add(new myProperty("CheckDirtMethod", "06.AOI"));
                            ////publicproperties.Add(new myProperty("CheckColorMethod", "06.AOI"));
                            //publicproperties.Add(new myProperty("IsNG", "06.AOI"));
                            //publicproperties.Add(new myProperty("DirtRatio", "06.AOI"));
                            //publicproperties.Add(new myProperty("DirtArea", "06.AOI"));
                            ////publicproperties.Add(new myProperty("ColorRatio", "06.AOI"));
                            ////publicproperties.Add(new myProperty("TotalColorRatio", "06.AOI"));

                            //publicproperties.Add(new myProperty("LTCorner", "06.AOI"));
                            //publicproperties.Add(new myProperty("RTCorner", "06.AOI"));
                            //publicproperties.Add(new myProperty("LBCorner", "06.AOI"));
                            //publicproperties.Add(new myProperty("RBCorner", "06.AOI"));

                            //publicproperties.Add(new myProperty("XDirWidtHeight", "06.AOI"));
                            //publicproperties.Add(new myProperty("YDirWidtHeight", "06.AOI"));

                            //publicproperties.Add(new myProperty("HeightMethod", "07.Height"));

                            //publicproperties.Add(new myProperty("GapMethod", "08.Gap"));
                            //publicproperties.Add(new myProperty("OffsetAngle", "08.Gap"));
                            //publicproperties.Add(new myProperty("OffsetToLeftRight", "08.Gap"));
                            //publicproperties.Add(new myProperty("OffsetToUPMax", "08.Gap"));
                            //publicproperties.Add(new myProperty("OffsetToUPMin", "08.Gap"));
                            //publicproperties.Add(new myProperty("A_Max", "08.Gap"));
                            //publicproperties.Add(new myProperty("A_Min", "08.Gap"));
                            //publicproperties.Add(new myProperty("B_Max", "08.Gap"));
                            //publicproperties.Add(new myProperty("B_Min", "08.Gap"));
                            //publicproperties.Add(new myProperty("C_Max", "08.Gap"));
                            //publicproperties.Add(new myProperty("C_Min", "08.Gap"));
                            //publicproperties.Add(new myProperty("D_Max", "08.Gap"));
                            //publicproperties.Add(new myProperty("D_Min", "08.Gap"));
                            //publicproperties.Add(new myProperty("E_Max", "08.Gap"));
                            //publicproperties.Add(new myProperty("E_Min", "08.Gap"));
                            //publicproperties.Add(new myProperty("F_Max", "08.Gap"));
                            //publicproperties.Add(new myProperty("F_Min", "08.Gap"));
                            //publicproperties.Add(new myProperty("G_Max", "08.Gap"));
                            //publicproperties.Add(new myProperty("G_Min", "08.Gap"));
                            //publicproperties.Add(new myProperty("H_Max", "08.Gap"));
                            //publicproperties.Add(new myProperty("H_Min", "08.Gap"));


                            //publicproperties.Add(new myProperty("STILTSMethod", "09.Stilts"));
                            //publicproperties.Add(new myProperty("StiltsOffSet", "09.Stilts"));
                            //publicproperties.Add(new myProperty("StiltsGrayValue", "09.Stilts"));
                            //publicproperties.Add(new myProperty("StiltsNOGrayValue", "09.Stilts"));

                            #endregion

                            publicproperties.Add(new myProperty("PADMethod", "10.PADCHECK"));
                            publicproperties.Add(new myProperty("PADOWidthRatio", "10.PADCHECK"));
                            publicproperties.Add(new myProperty("PADOHeightRatio", "10.PADCHECK"));
                            publicproperties.Add(new myProperty("PADOAreaRatio", "10.PADCHECK"));
                            publicproperties.Add(new myProperty("PADGrayThreshold", "10.PADCHECK"));
                            publicproperties.Add(new myProperty("PADBlobGrayThreshold", "10.PADCHECK"));
                            publicproperties.Add(new myProperty("PADChipInBlobGrayThreshold", "10.PADCHECK"));
                            publicproperties.Add(new myProperty("PADCheckDArea", "10.PADCHECK"));
                            publicproperties.Add(new myProperty("PADCheckDWidth", "10.PADCHECK"));
                            publicproperties.Add(new myProperty("PADCheckDHeight", "10.PADCHECK"));

                            publicproperties.Add(new myProperty("PADExtendX", "10.PADCHECK"));
                            publicproperties.Add(new myProperty("PADExtendY", "10.PADCHECK"));

                            publicproperties.Add(new myProperty("GlueMax", "10.PADCHECK"));
                            publicproperties.Add(new myProperty("GlueMin", "10.PADCHECK"));
                            publicproperties.Add(new myProperty("GlueCheck", "10.PADCHECK"));
                            publicproperties.Add(new myProperty("ChipDirlevel", "10.PADCHECK"));
                            publicproperties.Add(new myProperty("ChipGleCheck", "10.PADCHECK"));
                            publicproperties.Add(new myProperty("ChipFindWhite", "10.PADCHECK"));
                            publicproperties.Add(new myProperty("GLEFindWhite", "10.PADCHECK"));

                            publicproperties.Add(new myProperty("PADThresholdMode", "10.PADCHECK"));
                            publicproperties.Add(new myProperty("NoGlueThresholdValue", "10.PADCHECK"));
                            //publicproperties.Add(new myProperty("PADCalMode", "10.PADCHECK"));

                            publicproperties.Add(new myProperty("ChipNoGlueMode", "10.PADCHECK"));
                            publicproperties.Add(new myProperty("ChipNoHaveMode", "10.PADCHECK"));
                            publicproperties.Add(new myProperty("ChipNoHaveModeOpString", "10.PADCHECK"));

                            publicproperties.Add(new myProperty("PADChipSizeMode", "10.PADCHECK"));
                            publicproperties.Add(new myProperty("PADAICategory", "10.PADCHECK"));
                            publicproperties.Add(new myProperty("PadInspectMethod", "10.PADCHECK"));
                            publicproperties.Add(new myProperty("PADINSPECTOPString", "10.PADCHECK"));

                            publicproperties.Add(new myProperty("CalExtendX", "10.PADCHECK"));
                            publicproperties.Add(new myProperty("CalExtendY", "10.PADCHECK"));

                            publicproperties.Add(new myProperty("BloodFillValueRatio", "10.PADCHECK"));
                            publicproperties.Add(new myProperty("FourSideNoGluePassValue", "10.PADCHECK"));
                            publicproperties.Add(new myProperty("GlueChipSlotDir", "10.PADCHECK"));
                            publicproperties.Add(new myProperty("GlueMaxTop", "10.PADCHECK"));
                            publicproperties.Add(new myProperty("GlueMinTop", "10.PADCHECK"));
                            publicproperties.Add(new myProperty("GlueMaxBottom", "10.PADCHECK"));
                            publicproperties.Add(new myProperty("GlueMinBottom", "10.PADCHECK"));
                            publicproperties.Add(new myProperty("GlueMaxLeft", "10.PADCHECK"));
                            publicproperties.Add(new myProperty("GlueMinLeft", "10.PADCHECK"));
                            publicproperties.Add(new myProperty("GlueMaxRight", "10.PADCHECK"));
                            publicproperties.Add(new myProperty("GlueMinRight", "10.PADCHECK"));

                            publicproperties.Add(new myProperty("GlueTopBottomOffset", "10.PADCHECK"));
                            publicproperties.Add(new myProperty("GlueLeftRightOffset", "10.PADCHECK"));

                            publicproperties.Add(new myProperty("BlackCalExtendX", "10.PADCHECK"));
                            publicproperties.Add(new myProperty("BlackCalExtendY", "10.PADCHECK"));
                            publicproperties.Add(new myProperty("BlackOffsetX", "10.PADCHECK"));
                            publicproperties.Add(new myProperty("BlackOffsetY", "10.PADCHECK"));

                            publicproperties.Add(new myProperty("FontSize", "10.PADCHECK"));
                            publicproperties.Add(new myProperty("LineWidth", "10.PADCHECK"));

                            publicproperties.Add(new myProperty("GleWidthUpper", "10.PADCHECK"));
                            publicproperties.Add(new myProperty("GleWidthLower", "10.PADCHECK"));
                            publicproperties.Add(new myProperty("GleHeightUpper", "10.PADCHECK"));
                            publicproperties.Add(new myProperty("GleHeightLower", "10.PADCHECK"));
                            publicproperties.Add(new myProperty("GleAreaUpper", "10.PADCHECK"));
                            publicproperties.Add(new myProperty("GleAreaLower", "10.PADCHECK"));

                            publicproperties.Add(new myProperty("PADExtendOPString", "10.PADCHECK"));

                            break;

                        case OptionEnum.MAIN_X6:
                        case JetEazy.OptionEnum.MAIN_SERVICE:

                            switch (Universal.FACTORYNAME)
                            {
                                case FactoryName.DAGUI:
                                    break;
                                default:
                                    publicproperties.Add(new myProperty("OCRMethod", "05.OCR or Barcode", "字符或条码检测"));
                                    publicproperties.Add(new myProperty("OCRMappingMethod", "05.OCR or Barcode", "字符或条码检测"));
                                    publicproperties.Add(new myProperty("MappingTextIndex", "05.OCR or Barcode", "字符或条码检测"));
                                    break;
                            }

                            publicproperties.Add(new myProperty("InspectionMethod", "03.Inspection"));
                            publicproperties.Add(new myProperty("Inspection_A_B_Method", "03.Inspection"));
                            publicproperties.Add(new myProperty("IBCount", "03.Inspection"));
                            publicproperties.Add(new myProperty("IBArea", "03.Inspection"));
                            publicproperties.Add(new myProperty("IBTolerance", "03.Inspection"));

                            //publicproperties.Add(new myProperty("MeasureMethod", "04.Measure"));
                            //publicproperties.Add(new myProperty("MMOPString", "04.Measure"));

                            //publicproperties.Add(new myProperty("AOIMethod", "06.AOI"));
                            //publicproperties.Add(new myProperty("CheckDirtMethod", "06.AOI"));
                            ////publicproperties.Add(new myProperty("CheckColorMethod", "06.AOI"));
                            //publicproperties.Add(new myProperty("IsNG", "06.AOI"));
                            //publicproperties.Add(new myProperty("DirtRatio", "06.AOI"));
                            //publicproperties.Add(new myProperty("DirtArea", "06.AOI"));


                            break;

                        default:

                            #region OCR

                            publicproperties.Add(new myProperty("InspectionMethod", "03.Inspection"));
                            publicproperties.Add(new myProperty("Inspection_A_B_Method", "03.Inspection"));
                            publicproperties.Add(new myProperty("IBCount", "03.Inspection"));
                            publicproperties.Add(new myProperty("IBArea", "03.Inspection"));
                            publicproperties.Add(new myProperty("IBTolerance", "03.Inspection"));

                            publicproperties.Add(new myProperty("MeasureMethod", "04.Measure"));
                            //publicproperties.Add(new myProperty("MMTolerance", "04.Measure"));
                            publicproperties.Add(new myProperty("MMOPString", "04.Measure"));
                            //publicproperties.Add(new myProperty("MMMaxGap", "04.Measure"));
                            //publicproperties.Add(new myProperty("MMMinGap", "04.Measure"));
                            //publicproperties.Add(new myProperty("MMPixelGap", "04.Measure"));
                            //publicproperties.Add(new myProperty("MMHTRatio", "04.Measure"));
                            //publicproperties.Add(new myProperty("MMWholeRatio", "04.Measure"));

                            publicproperties.Add(new myProperty("OCRMethod", "05.OCR or Barcode", "字符或条码检测"));
                            publicproperties.Add(new myProperty("OCRMappingMethod", "05.OCR or Barcode", "字符或条码检测"));

                            publicproperties.Add(new myProperty("AOIMethod", "06.AOI"));
                            publicproperties.Add(new myProperty("CheckDirtMethod", "06.AOI"));
                            //publicproperties.Add(new myProperty("CheckColorMethod", "06.AOI"));
                            publicproperties.Add(new myProperty("IsNG", "06.AOI"));
                            publicproperties.Add(new myProperty("DirtRatio", "06.AOI"));
                            publicproperties.Add(new myProperty("DirtArea", "06.AOI"));
                            //publicproperties.Add(new myProperty("ColorRatio", "06.AOI"));
                            //publicproperties.Add(new myProperty("TotalColorRatio", "06.AOI"));

                            publicproperties.Add(new myProperty("LTCorner", "06.AOI"));
                            publicproperties.Add(new myProperty("RTCorner", "06.AOI"));
                            publicproperties.Add(new myProperty("LBCorner", "06.AOI"));
                            publicproperties.Add(new myProperty("RBCorner", "06.AOI"));

                            publicproperties.Add(new myProperty("XDirWidtHeight", "06.AOI"));
                            publicproperties.Add(new myProperty("YDirWidtHeight", "06.AOI"));

                            publicproperties.Add(new myProperty("HeightMethod", "07.Height"));

                            publicproperties.Add(new myProperty("GapMethod", "08.Gap"));
                            publicproperties.Add(new myProperty("OffsetAngle", "08.Gap"));
                            publicproperties.Add(new myProperty("OffsetToLeftRight", "08.Gap"));
                            publicproperties.Add(new myProperty("OffsetToUPMax", "08.Gap"));
                            publicproperties.Add(new myProperty("OffsetToUPMin", "08.Gap"));
                            publicproperties.Add(new myProperty("A_Max", "08.Gap"));
                            publicproperties.Add(new myProperty("A_Min", "08.Gap"));
                            publicproperties.Add(new myProperty("B_Max", "08.Gap"));
                            publicproperties.Add(new myProperty("B_Min", "08.Gap"));
                            publicproperties.Add(new myProperty("C_Max", "08.Gap"));
                            publicproperties.Add(new myProperty("C_Min", "08.Gap"));
                            publicproperties.Add(new myProperty("D_Max", "08.Gap"));
                            publicproperties.Add(new myProperty("D_Min", "08.Gap"));
                            publicproperties.Add(new myProperty("E_Max", "08.Gap"));
                            publicproperties.Add(new myProperty("E_Min", "08.Gap"));
                            publicproperties.Add(new myProperty("F_Max", "08.Gap"));
                            publicproperties.Add(new myProperty("F_Min", "08.Gap"));
                            publicproperties.Add(new myProperty("G_Max", "08.Gap"));
                            publicproperties.Add(new myProperty("G_Min", "08.Gap"));
                            publicproperties.Add(new myProperty("H_Max", "08.Gap"));
                            publicproperties.Add(new myProperty("H_Min", "08.Gap"));


                            publicproperties.Add(new myProperty("STILTSMethod", "09.Stilts"));
                            publicproperties.Add(new myProperty("StiltsOffSet", "09.Stilts"));
                            publicproperties.Add(new myProperty("StiltsGrayValue", "09.Stilts"));
                            publicproperties.Add(new myProperty("StiltsNOGrayValue", "09.Stilts"));

                            #endregion

                            break;

                    }

                    break;

                default:

                    #region OCR

                    publicproperties.Add(new myProperty("InspectionMethod", "03.Inspection"));
                    publicproperties.Add(new myProperty("Inspection_A_B_Method", "03.Inspection"));
                    publicproperties.Add(new myProperty("IBCount", "03.Inspection"));
                    publicproperties.Add(new myProperty("IBArea", "03.Inspection"));
                    publicproperties.Add(new myProperty("IBTolerance", "03.Inspection"));

                    publicproperties.Add(new myProperty("MeasureMethod", "04.Measure"));
                    //publicproperties.Add(new myProperty("MMTolerance", "04.Measure"));
                    publicproperties.Add(new myProperty("MMOPString", "04.Measure"));
                    //publicproperties.Add(new myProperty("MMMaxGap", "04.Measure"));
                    //publicproperties.Add(new myProperty("MMMinGap", "04.Measure"));
                    //publicproperties.Add(new myProperty("MMPixelGap", "04.Measure"));
                    //publicproperties.Add(new myProperty("MMHTRatio", "04.Measure"));
                    //publicproperties.Add(new myProperty("MMWholeRatio", "04.Measure"));

                    publicproperties.Add(new myProperty("OCRMethod", "05.OCR or Barcode", "字符或条码检测"));
                    publicproperties.Add(new myProperty("OCRMappingMethod", "05.OCR or Barcode", "字符或条码检测"));

                    publicproperties.Add(new myProperty("AOIMethod", "06.AOI"));
                    publicproperties.Add(new myProperty("CheckDirtMethod", "06.AOI"));
                    //publicproperties.Add(new myProperty("CheckColorMethod", "06.AOI"));
                    publicproperties.Add(new myProperty("IsNG", "06.AOI"));
                    publicproperties.Add(new myProperty("DirtRatio", "06.AOI"));
                    publicproperties.Add(new myProperty("DirtArea", "06.AOI"));
                    //publicproperties.Add(new myProperty("ColorRatio", "06.AOI"));
                    //publicproperties.Add(new myProperty("TotalColorRatio", "06.AOI"));

                    publicproperties.Add(new myProperty("LTCorner", "06.AOI"));
                    publicproperties.Add(new myProperty("RTCorner", "06.AOI"));
                    publicproperties.Add(new myProperty("LBCorner", "06.AOI"));
                    publicproperties.Add(new myProperty("RBCorner", "06.AOI"));

                    publicproperties.Add(new myProperty("XDirWidtHeight", "06.AOI"));
                    publicproperties.Add(new myProperty("YDirWidtHeight", "06.AOI"));

                    publicproperties.Add(new myProperty("HeightMethod", "07.Height"));

                    publicproperties.Add(new myProperty("GapMethod", "08.Gap"));
                    publicproperties.Add(new myProperty("OffsetAngle", "08.Gap"));
                    publicproperties.Add(new myProperty("OffsetToLeftRight", "08.Gap"));
                    publicproperties.Add(new myProperty("OffsetToUPMax", "08.Gap"));
                    publicproperties.Add(new myProperty("OffsetToUPMin", "08.Gap"));
                    publicproperties.Add(new myProperty("A_Max", "08.Gap"));
                    publicproperties.Add(new myProperty("A_Min", "08.Gap"));
                    publicproperties.Add(new myProperty("B_Max", "08.Gap"));
                    publicproperties.Add(new myProperty("B_Min", "08.Gap"));
                    publicproperties.Add(new myProperty("C_Max", "08.Gap"));
                    publicproperties.Add(new myProperty("C_Min", "08.Gap"));
                    publicproperties.Add(new myProperty("D_Max", "08.Gap"));
                    publicproperties.Add(new myProperty("D_Min", "08.Gap"));
                    publicproperties.Add(new myProperty("E_Max", "08.Gap"));
                    publicproperties.Add(new myProperty("E_Min", "08.Gap"));
                    publicproperties.Add(new myProperty("F_Max", "08.Gap"));
                    publicproperties.Add(new myProperty("F_Min", "08.Gap"));
                    publicproperties.Add(new myProperty("G_Max", "08.Gap"));
                    publicproperties.Add(new myProperty("G_Min", "08.Gap"));
                    publicproperties.Add(new myProperty("H_Max", "08.Gap"));
                    publicproperties.Add(new myProperty("H_Min", "08.Gap"));


                    publicproperties.Add(new myProperty("STILTSMethod", "09.Stilts"));
                    publicproperties.Add(new myProperty("StiltsOffSet", "09.Stilts"));
                    publicproperties.Add(new myProperty("StiltsGrayValue", "09.Stilts"));
                    publicproperties.Add(new myProperty("StiltsNOGrayValue", "09.Stilts"));

                    #endregion

                    break;

            }
        }

        public void GetNormal(NORMALClass normal)
        {
            Name = normal.Name;
            AliasName = normal.AliasName;
            Brightness = normal.Brightness;
            Contrast = normal.Contrast;
            MaskMethod = normal.MaskMethod;
            ExtendX = normal.ExtendX;
            ExtendY = normal.ExtendY;

            RelateASN = normal.RelateASN;
            RelateASNItem = normal.RelateASNItem;
            IsSeed = normal.IsSeed;
        }
        public void SetNormal(NORMALClass normal)
        {
            normal.AliasName = AliasName;
            normal.Brightness = Brightness;
            normal.Contrast = Contrast;
            normal.MaskMethod = MaskMethod;
            normal.ExtendX = ExtendX;
            normal.ExtendY = ExtendY;
            normal.RelateASN = RelateASN;
            normal.RelateASNItem = RelateASNItem;
            normal.IsSeed = IsSeed;
        }
        public void GetAlign(ALIGNClass align)
        {
            AlignMethod = align.AlignMethod;
            AlignMode = align.AlignMode;
            MTPSample = align.MTPSample;
            MTCannyAuto = align.MTCannyAuto;
            MTCannyH = align.MTCannyH;
            MTCannyL = align.MTCannyL;
            MTRotation = align.MTRotation;
            MTScaling = align.MTScaling;
            MTMaxOcc = align.MTMaxOcc;
            MTTolerance = align.MTTolerance;
            MTIsSubPixel = align.MTIsSubPixel;
            MTOffset = align.MTOffset;
            MTResolution = align.MTResolution;
            ABSAlignMethod = align.AbsAlignMode;
            ABSOffset = align.ABSOffset;
        }
        public void SetAlign(ALIGNClass align)
        {
            align.AlignMethod = AlignMethod;
            align.AlignMode = AlignMode;
            align.MTPSample = MTPSample;
            align.MTCannyAuto = MTCannyAuto;
            align.MTCannyH = MTCannyH;
            align.MTCannyL = MTCannyL;
            align.MTRotation = MTRotation;
            align.MTScaling = MTScaling;
            align.MTMaxOcc = MTMaxOcc;
            align.MTTolerance = MTTolerance;
            align.MTIsSubPixel = MTIsSubPixel;
            align.MTOffset = MTOffset;
            align.MTResolution = MTResolution;
            align.AbsAlignMode = ABSAlignMethod;
            align.ABSOffset = ABSOffset;
        }
        public void GetInspection(INSPECTIONClass inspection)
        {
            InspectionMethod = inspection.InspectionMethod;
            Inspection_A_B_Method = inspection.InspectionAB;
            IBCount = inspection.IBCount;
            IBArea = inspection.IBArea;
            IBTolerance = inspection.IBTolerance;

        }
        public void SetInspection(INSPECTIONClass inspection)
        {
            inspection.InspectionAB = Inspection_A_B_Method;
            inspection.InspectionMethod = InspectionMethod;
            inspection.IBCount = IBCount;
            inspection.IBArea = IBArea;
            inspection.IBTolerance = IBTolerance;
        }
        public void GetMeasure(MEASUREClass measure)
        {
            MeasureMethod = measure.MeasureMethod;
            //MMTolerance = measure.MMTolerance;
            MMOPString = measure.MeasureMethod.ToString() + "#" + measure.MMOPString;
            //MMMaxGap = measure.MMMaxGap;
            //MMMinGap = measure.MMMinGap;
            //MMPixelGap = measure.MMPixelGap;
            //MMHTRatio = measure.MMHTRatio;
            //MMWholeRatio = measure.MMWholeRatio;
        }
        public void SetMeasure(MEASUREClass measure)
        {
            measure.MeasureMethod = (MeasureMethodEnum)Enum.Parse(typeof(MeasureMethodEnum), MMOPString.Split('#')[0], true);
            //measure.MMTolerance= MMTolerance;
            measure.MMOPString = MMOPString.Split('#')[1];
            //measure.MMMaxGap= MMMaxGap;
            //measure.MMMinGap= MMMinGap;
            //measure.MMPixelGap= MMPixelGap;
            //measure.MMHTRatio= MMHTRatio;
            //measure.MMWholeRatio= MMWholeRatio;
        }
        public void GetOCR(OCRCheckClass ocr)
        {
            OCRMethod = ocr.OCRMethod;
            OCRMappingMethod = ocr.OCRMappingMethod;
            string[] strings = OCRMappingMethod.Split('#');
            MappingTextIndex = 0;
            if (strings.Length == 2)
            {
                bool bOK = int.TryParse(strings[1], out int indexvalue);
                if (bOK)
                    MappingTextIndex = indexvalue;

            }
            //else
            //{
            //    MappingTextIndex = 0;
            //}
            //MappingTextIndex = ocr.MappingTextIndex;
        }
        public void SetOCR(OCRCheckClass ocr)
        {
            ocr.OCRMethod = OCRMethod;
            ocr.OCRMappingMethod = OCRMappingMethod;
            string[] strings = ocr.OCRMappingMethod.Split('#');
            ocr.MappingTextIndex = 0;
            if (strings.Length == 2)
            {
                bool bOK = int.TryParse(strings[1], out int indexvalue);
                if (bOK)
                    ocr.MappingTextIndex = indexvalue;
            }
            //ocr.MappingTextIndex = MappingTextIndex;
        }
        public void GetAOI(AOIClass aoi)
        {
            AOIMethod = aoi.AOIMethod;
            CheckDirtMethod = aoi.CheckDirtMethod;
            //CheckColorMethod = aoi.CheckColorMethod;
            IsNG = aoi.IsNG;
            DirtRatio = aoi.DirtRatio;
            DirtArea = aoi.DirtArea;
            //ColorRatio = aoi.ColorRatio;
            //TotalColorRatio = aoi.TotalColorRatio;

            LTCorner = aoi.CornerArray[(int)CornerEnum.LT];
            RTCorner = aoi.CornerArray[(int)CornerEnum.RT];
            LBCorner = aoi.CornerArray[(int)CornerEnum.LB];
            RBCorner = aoi.CornerArray[(int)CornerEnum.RB];

            XDirWidtHeight = aoi.WHArray[(int)PositionEnum.XDir];
            YDirWidtHeight = aoi.WHArray[(int)PositionEnum.YDir];
        }
        public void SetAOI(AOIClass aoi)
        {
            aoi.AOIMethod = AOIMethod;
            aoi.CheckDirtMethod = CheckDirtMethod;
            //aoi.CheckColorMethod = CheckColorMethod;
            aoi.IsNG = IsNG;

            aoi.DirtRatio = DirtRatio;
            aoi.DirtArea = DirtArea;
            //aoi.ColorRatio = ColorRatio;
            //aoi.TotalColorRatio = TotalColorRatio;

            aoi.CornerArray[(int)CornerEnum.LT] = LTCorner;
            aoi.CornerArray[(int)CornerEnum.RT] = RTCorner;
            aoi.CornerArray[(int)CornerEnum.LB] = LBCorner;
            aoi.CornerArray[(int)CornerEnum.RB] = RBCorner;

            aoi.WHArray[(int)PositionEnum.XDir] = XDirWidtHeight;
            aoi.WHArray[(int)PositionEnum.YDir] = YDirWidtHeight;

        }
        public void GetHeight(HEIGHTClass heightpara)
        {
            HeightMethod = heightpara.HeightMethod;

        }
        public void SetHeight(HEIGHTClass heightpara)
        {
            heightpara.HeightMethod = HeightMethod;
        }
        public void GetGap(GAPClass gap)
        {
            GapMethod = gap.GapMethod;
            OffsetAngle = gap.OffsetAngle;
            OffsetToLeftRight = gap.OffsetToLeftRight;
            OffsetToUPMax = gap.OffsetToUPMax;
            OffsetToUPMin = gap.OffsetToUPMin;
            A_Max = gap.A_Max;
            A_Min = gap.A_Min;
            B_Max = gap.B_Max;
            B_Min = gap.B_Min;
            C_Max = gap.C_Max;
            C_Min = gap.C_Min;
            D_Max = gap.D_Max;
            D_Min = gap.D_Min;
            E_Max = gap.E_Max;
            E_Min = gap.E_Min;
            F_Max = gap.F_Max;
            F_Min = gap.F_Min;
            G_Max = gap.G_Max;
            G_Min = gap.G_Min;
            H_Max = gap.H_Max;
            H_Min = gap.H_Min;

        }
        public void SetGap(GAPClass gap)
        {
            gap.GapMethod = GapMethod;

            gap.OffsetAngle = OffsetAngle;
            gap.OffsetToLeftRight = OffsetToLeftRight;
            gap.OffsetToUPMax = OffsetToUPMax;
            gap.OffsetToUPMin = OffsetToUPMin;

            gap.A_Max = A_Max;
            gap.A_Min = A_Min;
            gap.B_Max = B_Max;
            gap.B_Min = B_Min;
            gap.C_Max = C_Max;
            gap.C_Min = C_Min;
            gap.D_Max = D_Max;
            gap.D_Min = D_Min;
            gap.E_Max = E_Max;
            gap.E_Min = E_Min;
            gap.F_Max = F_Max;
            gap.F_Min = F_Min;
            gap.G_Max = G_Max;
            gap.G_Min = G_Min;
            gap.H_Max = H_Max;
            gap.H_Min = H_Min;
        }


        public void GetStilts(StiltsClass Stilts)
        {
            STILTSMethod = Stilts.StiltsMethod;
            StiltsOffSet = Stilts.StiltsOffSet;
            StiltsGrayValue = Stilts.StiltsGrayValue;
            StiltsNOGrayValue = Stilts.StiltsNOGrayValue;
        }
        public void SetStilts(StiltsClass Stilts)
        {
            Stilts.StiltsMethod = STILTSMethod;

            Stilts.StiltsOffSet = StiltsOffSet;

            Stilts.StiltsGrayValue = StiltsGrayValue;
            Stilts.StiltsNOGrayValue = StiltsNOGrayValue;
        }

        public void GetPADCHECK(PADINSPECTClass PAD)
        {
            PADMethod = PAD.PADMethod;
            PADOWidthRatio = PAD.OWidthRatio;
            PADOHeightRatio = PAD.OHeightRatio;
            PADOAreaRatio = PAD.OAreaRatio;
            PADGrayThreshold = PAD.PADGrayThreshold;
            PADBlobGrayThreshold = PAD.PADBlobGrayThreshold;
            PADChipInBlobGrayThreshold = PAD.PADChipInBlobGrayThreshold;
            PADCheckDArea = PAD.CheckDArea;
            PADCheckDWidth = PAD.CheckDWidth;
            PADCheckDHeight = PAD.CheckDHeight;
            PADExtendX = PAD.ExtendX;
            PADExtendY = PAD.ExtendY;
            GlueMax = PAD.GlueMax;
            GlueMin = PAD.GlueMin;
            GlueCheck = PAD.GlueCheck;
            PADThresholdMode = PAD.PADThresholdMode;
            NoGlueThresholdValue = PAD.NoGlueThresholdValue;
            PADCalMode = PAD.PADCalMode;
            ChipNoGlueMode = PAD.ChipNoGlueMode;

            ChipNoHaveMode = PAD.ChipNoHaveMode;
            ChipNoHaveModeOpString = PAD.ChipNoHaveMode.ToString() + "#" + PAD.ChipNoHaveModeOpString;

            PADChipSizeMode = PAD.PADChipSizeMode;
            CalExtendX = PAD.CalExtendX;
            CalExtendY = PAD.CalExtendY;
            BloodFillValueRatio = PAD.BloodFillValueRatio;
            PADAICategory = PAD.PADAICategory;
            GlueChipSlotDir = PAD.GlueChipSlotDir;
            GlueMaxTop = PAD.GlueMaxTop;
            GlueMinTop = PAD.GlueMinTop;
            GlueMaxBottom = PAD.GlueMaxBottom;
            GlueMinBottom = PAD.GlueMinBottom;
            GlueMaxLeft = PAD.GlueMaxLeft;
            GlueMinLeft = PAD.GlueMinLeft;
            GlueMaxRight = PAD.GlueMaxRight;
            GlueMinRight = PAD.GlueMinRight;
            GlueTopBottomOffset = PAD.GlueTopBottomOffset;
            GlueLeftRightOffset = PAD.GlueLeftRightOffset;

            BlackCalExtendX = PAD.BlackCalExtendX;
            BlackCalExtendY = PAD.BlackCalExtendY;
            BlackOffsetX = PAD.BlackOffsetX;
            BlackOffsetY = PAD.BlackOffsetY;
            ChipDirLevel = PAD.ChipDirlevel;
            ChipGleCheck = PAD.ChipGleCheck;
            ChipFindWhite = PAD.ChipFindWhite;
            GLEFindWhite = PAD.GLEFindWhite;

            FontSize = PAD.FontSize;
            LineWidth = PAD.LineWidth;

            GleWidthUpper = PAD.GleWidthUpper;
            GleWidthLower = PAD.GleWidthLower;
            GleHeightUpper = PAD.GleHeightUpper;
            GleHeightLower = PAD.GleHeightLower;
            GleAreaUpper = PAD.GleAreaUpper;
            GleAreaLower = PAD.GleAreaLower;

            FourSideNoGluePassValue = PAD.FourSideNoGluePassValue;

            PadInspectMethod = PAD.PadInspectMethod;
            PADINSPECTOPString = PAD.PadInspectMethod.ToString() + "#" + PAD.PADINSPECTOPString;
            PADExtendOPString = PAD.PADExtendOPString;
        }
        public void SetPADCHECK(PADINSPECTClass PAD)
        {
            PAD.PADMethod = PADMethod;
            PAD.OWidthRatio = PADOWidthRatio;
            PAD.OHeightRatio = PADOHeightRatio;
            PAD.OAreaRatio = PADOAreaRatio;
            PAD.PADGrayThreshold = PADGrayThreshold;
            PAD.PADBlobGrayThreshold = PADBlobGrayThreshold;
            PAD.PADChipInBlobGrayThreshold = PADChipInBlobGrayThreshold;
            PAD.CheckDArea = PADCheckDArea;
            PAD.CheckDWidth = PADCheckDWidth;
            PAD.CheckDHeight = PADCheckDHeight;
            PAD.ExtendX = PADExtendX;
            PAD.ExtendY = PADExtendY;
            PAD.GlueMax = GlueMax;
            PAD.GlueMin = GlueMin;
            PAD.GlueCheck = GlueCheck;
            PAD.PADThresholdMode = PADThresholdMode;
            PAD.NoGlueThresholdValue = NoGlueThresholdValue;
            PAD.PADCalMode = PADCalMode;
            PAD.ChipNoGlueMode = ChipNoGlueMode;

            PAD.ChipNoHaveMode = (ChipNoHave)Enum.Parse(typeof(ChipNoHave), ChipNoHaveModeOpString.Split('#')[0], true);
            PAD.ChipNoHaveModeOpString = ChipNoHaveModeOpString.Split('#')[1];

            PAD.PADChipSizeMode = PADChipSizeMode;
            PAD.CalExtendX = CalExtendX;
            PAD.CalExtendY = CalExtendY;
            PAD.BloodFillValueRatio = BloodFillValueRatio;
            PAD.PADAICategory = PADAICategory;
            PAD.GlueChipSlotDir = GlueChipSlotDir;
            PAD.GlueMaxTop = GlueMaxTop;
            PAD.GlueMinTop = GlueMinTop;
            PAD.GlueMaxBottom = GlueMaxBottom;
            PAD.GlueMinBottom = GlueMinBottom;
            PAD.GlueMaxLeft = GlueMaxLeft;
            PAD.GlueMinLeft = GlueMinLeft;
            PAD.GlueMaxRight = GlueMaxRight;
            PAD.GlueMinRight = GlueMinRight;

            PAD.BlackCalExtendX = BlackCalExtendX;
            PAD.BlackCalExtendY = BlackCalExtendY;
            PAD.BlackOffsetX = BlackOffsetX;
            PAD.BlackOffsetY = BlackOffsetY;
            PAD.ChipDirlevel = ChipDirLevel;
            PAD.ChipGleCheck = ChipGleCheck;
            PAD.ChipFindWhite = ChipFindWhite;
            PAD.GLEFindWhite = GLEFindWhite;

            PAD.FontSize = FontSize;
            PAD.LineWidth = LineWidth;

            PAD.GleWidthUpper = GleWidthUpper;
            PAD.GleWidthLower = GleWidthLower;
            PAD.GleHeightUpper = GleHeightUpper;
            PAD.GleHeightLower = GleHeightLower;
            PAD.GleAreaUpper = GleAreaUpper;
            PAD.GleAreaLower = GleAreaLower;
            PAD.GlueTopBottomOffset = GlueTopBottomOffset;
            PAD.GlueLeftRightOffset = GlueLeftRightOffset;

            PAD.FourSideNoGluePassValue = FourSideNoGluePassValue;

            PAD.PadInspectMethod = (PadInspectMethodEnum)Enum.Parse(typeof(PadInspectMethodEnum), PADINSPECTOPString.Split('#')[0], true);
            PAD.PADINSPECTOPString = PADINSPECTOPString.Split('#')[1];
            PAD.PADExtendOPString = PADExtendOPString;
        }

    }

    public class ASSEMBLEClass : ASSEMBLEClass3
    {

        #region 常规参数

        [Category("01.Normal"), DefaultValue(""), ReadOnly(true)]
        [DisplayName("0.程式编号")]
        public override string Name
        {
            get; set;
        }
        [Category("01.Normal"), DefaultValue("")]
        [DisplayName("1.参数名称")]
        public override string AliasName
        {
            get; set;
        }
        [Category("01.Normal"), DefaultValue(0)]
        [Description("0 非必要.\r范围 -200 <-> 200, 默认 0")]
        [TypeConverter(typeof(NumericUpDownTypeConverter))]
        [Editor(typeof(NumericUpDownTypeEditor), typeof(UITypeEditor)), MinMax(-200, 200)]
        [DisplayName("2.亮度")]
        public override int Brightness
        {
            get; set;
        }
        [Category("01.Normal"), DefaultValue(0)]
        [Description("0 非必要.\r范围 -200 <-> 200, 默认 0")]
        [TypeConverter(typeof(NumericUpDownTypeConverter))]
        [Editor(typeof(NumericUpDownTypeEditor), typeof(UITypeEditor)), MinMax(-200, 200)]
        [DisplayName("3.对比度")]
        public override int Contrast
        {
            get; set;
        }
        [Category("01.Normal"), DefaultValue(MaskMethodEnum.NONE)]
        [Description("相关 Mask 方式.")]
        [DisplayName("4.遮挡方式")]
        [TypeConverter(typeof(JzEnumConverter))]
        public override MaskMethodEnum MaskMethod
        {
            get; set;
        }
        [Category("01.Normal"), DefaultValue(20)]
        [Description("0 非必要.\r范围 0 <-> 1000, 默认 20 单位像素pixel")]
        [TypeConverter(typeof(NumericUpDownTypeConverter))]
        [Editor(typeof(NumericUpDownTypeEditor), typeof(UITypeEditor)), MinMax(0, 1000)]
        [DisplayName("5.横向外扩")]
        public override int ExtendX
        {
            get; set;
        }
        [Category("01.Normal"), DefaultValue(20)]
        [Description("0 非必要.\r范围 0 <-> 1000, 默认 20 单位像素pixel")]
        [TypeConverter(typeof(NumericUpDownTypeConverter))]
        [Editor(typeof(NumericUpDownTypeEditor), typeof(UITypeEditor)), MinMax(0, 1000)]
        [DisplayName("6.纵向外扩")]
        public override int ExtendY
        {
            get; set;
        }
        [Category("01.Normal"), DefaultValue("None")]
        [Description("关联 ASN 名称")]
        [Browsable(true)]
        [TypeConverter(typeof(JzASNStringConverter))]
        [DisplayName("7.ASN项目")]
        public override string RelateASN
        {
            get; set;
        }
        [Category("01.Normal"), DefaultValue("None")]
        [Description("关联 ASN 项目名称")]
        [Browsable(true)]
        [TypeConverter(typeof(JzASNItemStringConverter))]
        [DisplayName("8.ASN名称")]
        public override string RelateASNItem
        {
            get; set;
        }

        [Category("01.Normal"), DefaultValue("false")]
        [Description("IsSeed")]
        [Browsable(true)]
        //[TypeConverter(typeof(JzASNItemStringConverter))]
        [DisplayName("Seed(种子)")]
        public override bool IsSeed
        {
            get; set;
        }

        #endregion

        #region Align Factor

        [Category("02.Align"), DefaultValue(AlignMethodEnum.NONE)]
        [Description("选择定位模式")]
        [DisplayName("0.定位方法")]
        [TypeConverter(typeof(JzEnumConverter))]
        public override AlignMethodEnum AlignMethod
        {
            get; set;
        }

        [Category("02.Align"), DefaultValue(AlignModeEnum.AREA)]
        [Description("选择定位模式.")]
        [DisplayName("1.定位模式")]
        [TypeConverter(typeof(JzEnumConverter))]
        public override AlignModeEnum AlignMode
        {
            get; set;
        }
        [Category("02.Align"), DefaultValue(0.1)]
        [Description("筛选分数 数值越大越严格.\r范围 0 <-> 1.,  默认 0.1 此值是百分比无单位")]
        [DisplayName("2.容许值")]
        [TypeConverter(typeof(NumericUpDownTypeConverter))]
        [Editor(typeof(NumericUpDownTypeEditor), typeof(UITypeEditor)), MinMax(0f, 1f, 0.1f, 2)]
        public override float MTTolerance
        {
            get; set;
        }

        [Category("02.Align"), DefaultValue(20)]
        [Description("定位采样大小,0表示最小.\r范围 0 <-> 5000, 默认 35")]
        [DisplayName("3.采样大小")]
        [TypeConverter(typeof(NumericUpDownTypeConverter))]
        [Editor(typeof(NumericUpDownTypeEditor), typeof(UITypeEditor)), MinMax(-10f, 5000, 1f, 1)]
        public override float MTPSample
        {
            get; set;
        }

       
        [Category("02.Align"), DefaultValue(20)]
        [Description("允许的角度.\r范围 0 <-> 180.,  默认 20.00")]
        [DisplayName("4.角度")]
        [TypeConverter(typeof(NumericUpDownTypeConverter))]
        [Editor(typeof(NumericUpDownTypeEditor), typeof(UITypeEditor)), MinMax(0f, 180f, 1f, 2)]
        public override float MTRotation
        {
            get; set;
        }

        [Category("02.Align"), DefaultValue(0)]
        [Description("允许的缩放倍数.\r范围 0% <-> 100%.,  默认 0.00")]
        [DisplayName("5.缩放")]
        [TypeConverter(typeof(NumericUpDownTypeConverter))]
        [Editor(typeof(NumericUpDownTypeEditor), typeof(UITypeEditor)), MinMax(0f, 100f, 1f, 2)]
        public override float MTScaling
        {
            get; set;
        }
        [Category("02.Align"), DefaultValue(1)]
        [Description("寻找的最大个数.\r范围 1 <-> 500,  默认 1")]
        [DisplayName("6.个数")]
        [TypeConverter(typeof(NumericUpDownTypeConverter))]
        [Editor(typeof(NumericUpDownTypeEditor), typeof(UITypeEditor)), MinMax(1, 500)]
        public override int MTMaxOcc
        {
            get; set;
        }
      
        [Category("02.Align"), DefaultValue(false)] //No Use Now
        [DisplayName("7.亚像素")]
        public override bool MTIsSubPixel
        {
            get; set;
        }
        [Category("02.Align"), DefaultValue(0.5)]
        [Description("允许的偏移量.\r范围 0 <-> 100.,  默认 0.5 单位毫米mm")]
        [DisplayName("8.偏移")]
        [TypeConverter(typeof(NumericUpDownTypeConverter))]
        [Editor(typeof(NumericUpDownTypeEditor), typeof(UITypeEditor)), MinMax(0f, 100f, 0.1f, 3)]
        public override float MTOffset
        {
            get; set;
        }
        [Category("02.Align"), DefaultValue(0.02)]
        [Description("一个像素代表的大小(分辩率).\r范围 0 <-> 100.,  默认 0.02 单位毫米/像素  mm/pixel")]
        [DisplayName("9.分辩率")]
        [TypeConverter(typeof(NumericUpDownTypeConverter))]
        [Editor(typeof(NumericUpDownTypeEditor), typeof(UITypeEditor)), MinMax(0f, 100f, 0.1f, 3)]
        public override float MTResolution
        {
            get; set;
        }

        [Category("02.Align"), DefaultValue(true)]
        [Description("Canny边缘检测算法 , 默认打开")]
        [DisplayName("A.边缘算法")]
        [TypeConverter(typeof(bool))]
        public override bool MTCannyAuto
        {
            get; set;
        }

        [Category("02.Align"), DefaultValue(200)]
        [Description("Canny算法最大值\r范围 0 <-> 200, 默认 200")]
        [DisplayName("A.最大值.")]
        [TypeConverter(typeof(NumericUpDownTypeConverter))]
        [Editor(typeof(NumericUpDownTypeEditor), typeof(UITypeEditor)), MinMax(0, 255)]
        public override int MTCannyH
        {
            get; set;
        }

        [Category("02.Align"), DefaultValue(128)]
        [Description("Canny算法最小值.\r范围 0 <-> 200,  默认 128")]
        [DisplayName("A.最小值.")]
        [TypeConverter(typeof(NumericUpDownTypeConverter))]
        [Editor(typeof(NumericUpDownTypeEditor), typeof(UITypeEditor)), MinMax(0, 255)]
        public override int MTCannyL
        {
            get; set;
        }

        [Category("02.Align"), DefaultValue(AlignMethodEnum.NONE)]
        [Description("选择定位模式")]
        [DisplayName("B.绝对定位方法")]
        [TypeConverter(typeof(JzEnumConverter))]
        public override AbsoluteAlignEnum ABSAlignMethod
        {
            get; set;
        }
        [Category("02.Align"), DefaultValue(0.2)]
        [Description("允许的偏移量.\r范围 0 <-> 100.,  默认 0.2 单位毫米mm")]
        [DisplayName("B.绝对偏移")]
        [TypeConverter(typeof(NumericUpDownTypeConverter))]
        [Editor(typeof(NumericUpDownTypeEditor), typeof(UITypeEditor)), MinMax(0f, 100f, 0.1f, 3)]
        public override float ABSOffset
        {
            get; set;
        }
        #endregion

        #region Inspection Factor

        [Category("03.Inspection"), DefaultValue(InspectionMethodEnum.NONE)]
        [Description("选用用检验方法.")]
        [DisplayName("0.方法")]
        [TypeConverter(typeof(InspectionMethodEnum))]
        public override InspectionMethodEnum InspectionMethod
        {
            get; set;
        }
        [Category("03.Inspection"), DefaultValue(3)]
        [Description("检测最允许的最大个数\r范围 -1 <-> 1000.,  默认 3, -1 为禁用 单位个")]
        [DisplayName("2.个数")]
        [TypeConverter(typeof(NumericUpDownTypeConverter))]
        [Editor(typeof(NumericUpDownTypeEditor), typeof(UITypeEditor)), MinMax(-1, 1000)]
        public override int IBCount
        {
            get; set;
        }
        [Category("03.Inspection"), DefaultValue(5)]
        [Description("检测时允许的最大面积\r范围 1 <-> 1000.,  默认 5 单位pixel ")]
        [DisplayName("1.面积")]
        [TypeConverter(typeof(NumericUpDownTypeConverter))]
        [Editor(typeof(NumericUpDownTypeEditor), typeof(UITypeEditor)), MinMax(1, 1000)]
        public override int IBArea
        {
            get; set;
        }
        [Category("03.Inspection"), DefaultValue(15)]
        [Description("像素容许值.\r范围 0 <-> 255,  默认 15 图像灰阶值")]
        [DisplayName("3.容许值")]
        [TypeConverter(typeof(NumericUpDownTypeConverter))]
        [Editor(typeof(NumericUpDownTypeEditor), typeof(UITypeEditor)), MinMax(0, 255)]
        public override int IBTolerance
        {
            get; set;
        }


        #endregion

        #region Measure Factor
        [Category("04.Measure"), DefaultValue(MeasureMethodEnum.NONE), ReadOnly(true)]
        //[Category("04.Measure")]
        [Description("选用量测的方式.")]
        [DisplayName("0.量测方式")]
        public override MeasureMethodEnum MeasureMethod
        {
            get; set;
        }
        [Category("04.Measure"), DefaultValue(10)]
        [Description("测量公差.\r范围 0% <-> 100%.,  默认 10(%)")]
        [DisplayName("1.公差")]
        [TypeConverter(typeof(NumericUpDownTypeConverter))]
        [Editor(typeof(NumericUpDownTypeEditor), typeof(UITypeEditor)), MinMax(0f, 100f, 1f, 2)]
        public override float MMTolerance
        {
            get; set;
        }
        [Category("04.Measure"), DefaultValue(""), ReadOnly(false)]
        [Description("选择量测的相应操作")]
        [DisplayName("2.量测参数")]
        [Editor(typeof(GetMeasureDataPropertyEditor), typeof(UITypeEditor))]
        public override string MMOPString
        {
            get; set;
        }
        [Category("04.Measure"), DefaultValue(5)]
        [Description("设定最大的间隔.\r范围 0 <-> 1000.,  默认 is 5")]
        [DisplayName("3.最大间隔")]
        [TypeConverter(typeof(NumericUpDownTypeConverter))]
        [Editor(typeof(NumericUpDownTypeEditor), typeof(UITypeEditor)), MinMax(0f, 1000f, 1f, 2)]
        public override float MMMaxGap
        {
            get; set;
        }
        [Category("04.Measure"), DefaultValue(5)]
        [Description("设定最小的间隔.\r范围 0 <-> 1000.,  默认 is 5")]
        [DisplayName("4.最小间隔")]
        [TypeConverter(typeof(NumericUpDownTypeConverter))]
        [Editor(typeof(NumericUpDownTypeEditor), typeof(UITypeEditor)), MinMax(0f, 1000f, 1f, 2)]
        public override float MMMinGap
        {
            get; set;
        }
        [Category("04.Measure"), DefaultValue(5)]
        [Description("像素间隔.\r范围 0 <-> 100.,  默认 5")]
        [DisplayName("5.像素间隔")]
        [TypeConverter(typeof(NumericUpDownTypeConverter))]
        [Editor(typeof(NumericUpDownTypeEditor), typeof(UITypeEditor)), MinMax(0, 100)]
        public override int MMPixelGap
        {
            get; set;
        }
        [Category("04.Measure"), DefaultValue(0)]
        [Description("允许的头尾比例.\r范围 0% <-> 100%.,  默认 is 5%")]
        [DisplayName("6.头尾比例")]
        [TypeConverter(typeof(NumericUpDownTypeConverter))]
        [Editor(typeof(NumericUpDownTypeEditor), typeof(UITypeEditor)), MinMax(0f, 100f, 5f, 2)]
        public override float MMHTRatio
        {
            get; set;
        }
        [Category("04.Measure"), DefaultValue(0)]
        [Description("允许的整体比例.\r范围 0 <-> 1000.,  默认 is 5")]
        [DisplayName("7.整体比例")]
        [TypeConverter(typeof(NumericUpDownTypeConverter))]
        [Editor(typeof(NumericUpDownTypeEditor), typeof(UITypeEditor)), MinMax(0f, 100f, 5f, 2)]
        public override float MMWholeRatio
        {
            get; set;
        }

        #endregion

        #region OCR Factor
        [Category("05.字符和条码读取"), DefaultValue(OCRMethodEnum.NONE)]
        [Description("设定使用OCR或读取条码的方法。")]
        [DisplayName("0.设定方法")]
        [TypeConverter(typeof(JzEnumConverter))]
        public override OCRMethodEnum OCRMethod { get => base.OCRMethod; set => base.OCRMethod = value; }


        [Category("05.字符和条码读取"), DefaultValue("None")]
        [Description("字符读取设定")]
        [Browsable(true)]
        [TypeConverter(typeof(JzOCRStringConverter))]
        [DisplayName("1.参数选择")]
        public override string OCRMappingMethod { get => base.OCRMappingMethod; set => base.OCRMappingMethod = value; }


        [Category("05.字符和条码读取"), DefaultValue(0)]
        [Description("即打印内容的序号")]
        [Browsable(true)]
        [ReadOnly(false)]
        [TypeConverter(typeof(NumericUpDownTypeConverter))]
        [Editor(typeof(NumericUpDownTypeEditor), typeof(UITypeEditor)), MinMax(0, 1000)]
        [DisplayName("2.字符索引")]
        public override int MappingTextIndex
        {
            get; set;
        }

        #endregion

        #region AOI Factor

        [Category("06.AOI"), DefaultValue(AOIMethodEnum.NONE)]
        [Description("选择使用AOI的方法.")]
        [DisplayName("0.AOI方法")]
        [TypeConverter(typeof(JzEnumConverter))]
        public override AOIMethodEnum AOIMethod
        {
            get; set;
        }

        [Category("06.AOI"), DefaultValue(CheckDirtMethodEnum.NONE)]
        [Description("检查脏污的方法.")]
        [DisplayName("1.检查模式")]
        [TypeConverter(typeof(JzEnumConverter))]
        public override CheckDirtMethodEnum CheckDirtMethod
        {
            get; set;
        }

        [Category("06.AOI"), DefaultValue(UselessMethodEnum.NONE)]
        [Description("检查颜色的模式.")]
        [DisplayName("2.颜色检查")]
        [TypeConverter(typeof(JzEnumConverter))]
        public override UselessMethodEnum CheckColorMethod
        {
            get; set;
        }
        [Category("06.AOI"), DefaultValue(false)]
        [Description("检查结果反转.")]
        [DisplayName("3.结果反转")]
        public override bool IsNG
        {
            get; set;
        }
        [Category("06.AOI"), DefaultValue(20)]
        [Description("脏污百分比.\r范围 0% <-> 100%.,  默认 20(%)")]
        [DisplayName("4.脏污比率")]
        [TypeConverter(typeof(NumericUpDownTypeConverter))]
        [Editor(typeof(NumericUpDownTypeEditor), typeof(UITypeEditor)), MinMax(0f, 100f, 0.1f, 3)]
        public override float DirtRatio
        {
            get; set;
        }
        [Category("06.AOI"), DefaultValue(10)]
        [Description("脏污面积.\r范围 0 <-> 200.,  默认 10")]
        [DisplayName("5.脏污面积")]
        [TypeConverter(typeof(NumericUpDownTypeConverter))]
        [Editor(typeof(NumericUpDownTypeEditor), typeof(UITypeEditor)), MinMax(0, 100)]
        public override int DirtArea
        {
            get; set;
        }
        [Category("06.AOI"), DefaultValue(40)]
        [Description("彩色比例.\r范围 0% <-> 100%.,  默认 is 40(%)")]
        [DisplayName("6.彩色比例")]
        [TypeConverter(typeof(NumericUpDownTypeConverter))]
        [Editor(typeof(NumericUpDownTypeEditor), typeof(UITypeEditor)), MinMax(0f, 100f, 0.1f, 2)]
        public override float ColorRatio
        {
            get; set;
        }
        [Category("06.AOI"), DefaultValue(0.02)]
        [Description("总计彩色比例.\r范围 0% <-> 100%.,  默认 is 70(%)")]
        [DisplayName("7.总计比例")]
        [TypeConverter(typeof(NumericUpDownTypeConverter))]
        [Editor(typeof(NumericUpDownTypeEditor), typeof(UITypeEditor)), MinMax(0f, 100f, 0.1f, 3)]
        public override float TotalColorRatio
        {
            get; set;
        }

        [Category("06.AOI")]
        [Description("键帽 左上角 参数设定")]
        [DisplayName("8.左上角")]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public override CornerClass LTCorner
        {
            get; set;
        }
        [Category("06.AOI")]
        [Description("键帽 右上角 参数设定")]
        [DisplayName("9.右上角")]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public override CornerClass RTCorner
        {
            get; set;
        }
        [Category("06.AOI")]
        [Description("键帽 左下角 参数设定")]
        [DisplayName("A.左下角")]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public override CornerClass LBCorner
        {
            get; set;
        }
        [Category("06.AOI")]
        [Description("键帽 右下角 参数设定")]
        [DisplayName("B.右下角")]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public override CornerClass RBCorner
        {
            get; set;
        }

        [Category("06.AOI")]
        [Description("宽度检查设置")]
        [DisplayName("C.宽度设置")]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public override WHClass XDirWidtHeight
        {
            get; set;
        }

        [Category("06.AOI")]
        [Description("高度检查设置")]
        [DisplayName("D.高度设置")]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public override WHClass YDirWidtHeight
        {
            get; set;
        }


        #endregion
        #region HEIGHT Factor

        [Category("07.Height"), DefaultValue(HeightMethodEnum.NONE)]
        [Description("选择高度量测设定.")]
        [DisplayName("0.高度模式")]
        [TypeConverter(typeof(JzEnumConverter))]
        public override HeightMethodEnum HeightMethod
        {
            get; set;
        }

        #endregion

        #region GAP Factor


        [Category("08.Gap"), DefaultValue(GapMethodEnum.NONE)]
        [Description("选择间隔模式.")]
        [DisplayName("0.间隔模式2")]
        [TypeConverter(typeof(JzEnumConverter))]
        public override GapMethodEnum GapMethod
        {
            get; set;
        }

        [Category("08.Gap"), DefaultValue(5)]
        [Description("充许的最大偏移角度，单位：度。")]
        [DisplayName("1.偏移角度")]
        [TypeConverter(typeof(NumericUpDownTypeConverter))]
        [Editor(typeof(NumericUpDownTypeEditor), typeof(UITypeEditor)), MinMax(0f, 180f, 0.1f, 3)]
        public override float OffsetAngle
        {
            get; set;
        }

        [Category("08.Gap"), DefaultValue(30)]
        [Description("距离横边最大的距离 单位毫米。")]
        [DisplayName("2.最大距离")]
        [TypeConverter(typeof(NumericUpDownTypeConverter))]
        [Editor(typeof(NumericUpDownTypeEditor), typeof(UITypeEditor)), MinMax(0f, 1000f, 0.1f, 3)]
        public override float OffsetToUPMax
        {
            get; set;
        }
        [Category("08.Gap"), DefaultValue(20)]
        [Description("距离横边最小的距离 单位毫米。")]
        [DisplayName("3.最小距离")]
        [TypeConverter(typeof(NumericUpDownTypeConverter))]
        [Editor(typeof(NumericUpDownTypeEditor), typeof(UITypeEditor)), MinMax(0f, 1000f, 0.1f, 3)]
        public override float OffsetToUPMin
        {
            get; set;
        }

        [Category("08.Gap"), DefaultValue(10)]
        [Description("镭雕距离左边和右边最大差异 单位：毫米。")]
        [DisplayName("4.左右差异")]
        [TypeConverter(typeof(NumericUpDownTypeConverter))]
        [Editor(typeof(NumericUpDownTypeEditor), typeof(UITypeEditor)), MinMax(0f, 100f, 0.1f, 3)]
        public override float OffsetToLeftRight
        {
            get; set;
        }
        

        /// <summary>
        /// A位置最大值
        /// </summary>
        [Category("08.Gap"), DefaultValue(100)]
        [Description("A位置最大值 单位毫米。")]
        [DisplayName("5.A最大值")]
        [TypeConverter(typeof(NumericUpDownTypeConverter))]
        [Editor(typeof(NumericUpDownTypeEditor), typeof(UITypeEditor)), MinMax(0f, 1000f, 0.1f, 3)]
        public override float A_Max
        {
            get; set;
        }
        /// <summary>
        /// A位置最小值
        /// </summary>
        [Category("08.Gap"), DefaultValue(100)]
        [Description("A位置最小值 单位毫米。")]
        [DisplayName("6.A最小值")]
        [TypeConverter(typeof(NumericUpDownTypeConverter))]
        [Editor(typeof(NumericUpDownTypeEditor), typeof(UITypeEditor)), MinMax(0f, 1000f, 0.1f, 3)]
        public override float A_Min
        {
            get; set;
        }
        /// <summary>
        /// B位置最大值
        /// </summary>
        [Category("08.Gap"), DefaultValue(100)]
        [Description("B位置最大值 单位毫米。")]
        [DisplayName("7.B最大值")]
        [TypeConverter(typeof(NumericUpDownTypeConverter))]
        [Editor(typeof(NumericUpDownTypeEditor), typeof(UITypeEditor)), MinMax(0f, 1000f, 0.1f, 3)]
        public override float B_Max
        {
            get; set;
        }
        /// <summary>
        /// B位置最小值
        /// </summary>
        [Category("08.Gap"), DefaultValue(100)]
        [Description("B位置最小值 单位毫米。")]
        [DisplayName("8.B最小值")]
        [TypeConverter(typeof(NumericUpDownTypeConverter))]
        [Editor(typeof(NumericUpDownTypeEditor), typeof(UITypeEditor)), MinMax(0f, 1000f, 0.1f, 3)]
        public override float B_Min
        {
            get; set;
        }
        /// <summary>
        /// C位置最大值
        /// </summary>
        [Category("08.Gap"), DefaultValue(100)]
        [Description("C位置最大值 单位毫米。")]
        [DisplayName("9.C最大值")]
        [TypeConverter(typeof(NumericUpDownTypeConverter))]
        [Editor(typeof(NumericUpDownTypeEditor), typeof(UITypeEditor)), MinMax(0f, 1000f, 0.1f, 3)]
        public override float C_Max
        {
            get; set;
        }
        /// <summary>
        /// C位置最小值
        /// </summary>
        [Category("08.Gap"), DefaultValue(100)]
        [Description("C位置最小值 单位毫米。")]
        [DisplayName("a.C最小值")]
        [TypeConverter(typeof(NumericUpDownTypeConverter))]
        [Editor(typeof(NumericUpDownTypeEditor), typeof(UITypeEditor)), MinMax(0f, 1000f, 0.1f, 3)]
        public override float C_Min
        {
            get; set;
        }
        /// <summary>
        /// D位置最大值
        /// </summary>
        [Category("08.Gap"), DefaultValue(100)]
        [Description("D位置最大值 单位毫米。")]
        [DisplayName("b.D最大值")]
        [TypeConverter(typeof(NumericUpDownTypeConverter))]
        [Editor(typeof(NumericUpDownTypeEditor), typeof(UITypeEditor)), MinMax(0f, 1000f, 0.1f, 3)]
        public override float D_Max
        {
            get; set;
        }
        /// <summary>
        /// D位置最小值
        /// </summary>
        [Category("08.Gap"), DefaultValue(100)]
        [Description("D位置最小值 单位毫米。")]
        [DisplayName("c.D最小值")]
        [TypeConverter(typeof(NumericUpDownTypeConverter))]
        [Editor(typeof(NumericUpDownTypeEditor), typeof(UITypeEditor)), MinMax(0f, 1000f, 0.1f, 3)]
        public override float D_Min
        {
            get; set;
        }
        /// <summary>
        /// E位置最大值
        /// </summary>
        [Category("08.Gap"), DefaultValue(100)]
        [Description("E位置最大值 单位毫米。")]
        [DisplayName("d.E最大值")]
        [TypeConverter(typeof(NumericUpDownTypeConverter))]
        [Editor(typeof(NumericUpDownTypeEditor), typeof(UITypeEditor)), MinMax(0f, 1000f, 0.1f, 3)]
        public override float E_Max
        {
            get; set;
        }
        /// <summary>
        /// E位置最小值
        /// </summary>
        [Category("08.Gap"), DefaultValue(100)]
        [Description("E位置最小值 单位毫米。")]
        [DisplayName("e.E最小值")]
        [TypeConverter(typeof(NumericUpDownTypeConverter))]
        [Editor(typeof(NumericUpDownTypeEditor), typeof(UITypeEditor)), MinMax(0f, 1000f, 0.1f, 3)]
        public override float E_Min
        {
            get; set;
        }
        /// <summary>
        /// F位置最大值
        /// </summary>
        [Category("08.Gap"), DefaultValue(100)]
        [Description("F位置最大值 单位毫米。")]
        [DisplayName("f.F最大值")]
        [TypeConverter(typeof(NumericUpDownTypeConverter))]
        [Editor(typeof(NumericUpDownTypeEditor), typeof(UITypeEditor)), MinMax(0f, 1000f, 0.1f, 3)]
        public override float F_Max
        {
            get; set;
        }
        /// <summary>
        /// F位置最小值
        /// </summary>
        [Category("08.Gap"), DefaultValue(100)]
        [Description("F位置最小值 单位毫米。")]
        [DisplayName("g.F最小值")]
        [TypeConverter(typeof(NumericUpDownTypeConverter))]
        [Editor(typeof(NumericUpDownTypeEditor), typeof(UITypeEditor)), MinMax(0f, 1000f, 0.1f, 3)]
        public override float F_Min
        {
            get; set;
        }
        /// <summary>
        /// G位置最大值
        /// </summary>
        [Category("08.Gap"), DefaultValue(100)]
        [Description("G位置最大值 单位毫米。")]
        [DisplayName("h.G最大值")]
        [TypeConverter(typeof(NumericUpDownTypeConverter))]
        [Editor(typeof(NumericUpDownTypeEditor), typeof(UITypeEditor)), MinMax(0f, 1000f, 0.1f, 3)]
        public override float G_Max
        {
            get; set;
        }
        /// <summary>
        /// G位置最小值
        /// </summary>
        [Category("08.Gap"), DefaultValue(100)]
        [Description("G位置最小值 单位毫米。")]
        [DisplayName("i.G最小值")]
        [TypeConverter(typeof(NumericUpDownTypeConverter))]
        [Editor(typeof(NumericUpDownTypeEditor), typeof(UITypeEditor)), MinMax(0f, 1000f, 0.1f, 3)]
        public override float G_Min
        {
            get; set;
        }
        /// <summary>
        /// H位置最大值
        /// </summary>
        [Category("08.Gap"), DefaultValue(100)]
        [Description("H位置最大值 单位毫米。")]
        [DisplayName("j.H最大值")]
        [TypeConverter(typeof(NumericUpDownTypeConverter))]
        [Editor(typeof(NumericUpDownTypeEditor), typeof(UITypeEditor)), MinMax(0f, 1000f, 0.1f, 3)]
        public override float H_Max
        {
            get; set;
        }
        /// <summary>
        /// H位置最小值
        /// </summary>
        [Category("08.Gap"), DefaultValue(100)]
        [Description("H位置最小值 单位毫米。")]
        [DisplayName("k.H最小值")]
        [TypeConverter(typeof(NumericUpDownTypeConverter))]
        [Editor(typeof(NumericUpDownTypeEditor), typeof(UITypeEditor)), MinMax(0f, 1000f, 0.1f, 3)]
        public override float H_Min
        {
            get; set;
        }
        #endregion

        #region STILTS Factor


        [Category("09.Stilts"), DefaultValue(STILTSMethodEnum.NONE)]
        [Description("是否检查高跷模式.")]
        [DisplayName("0.是否检查高跷")]
        [TypeConverter(typeof(JzEnumConverter))]
        public override STILTSMethodEnum STILTSMethod
        {
            get; set;
        }

        [Category("09.Stilts"), DefaultValue(5)]
        [Description("充许的最大长度，单位：像素。")]
        [DisplayName("1.高跷的长度")]
        [TypeConverter(typeof(NumericUpDownTypeConverter))]
        [Editor(typeof(NumericUpDownTypeEditor), typeof(UITypeEditor)), MinMax(0f, 180f, 0.1f, 3)]
        public override int StiltsOffSet
        {
            get; set;
        }


        [Category("09.Stilts"), DefaultValue(70)]
        [Description("阴影的灰度值，范围0-255。")]
        [DisplayName("2.阴影灰度")]
        [TypeConverter(typeof(NumericUpDownTypeConverter))]
        [Editor(typeof(NumericUpDownTypeEditor), typeof(UITypeEditor)), MinMax(1, 255f, 1, 0)]
        public override int StiltsGrayValue
        {
            get; set;
        }
        [Category("09.Stilts"), DefaultValue(70)]
        [Description("非阴影区最小灰度值，范围0-255。")]
        [DisplayName("3.非阴影最小灰度")]
        [TypeConverter(typeof(NumericUpDownTypeConverter))]
        [Editor(typeof(NumericUpDownTypeEditor), typeof(UITypeEditor)), MinMax(1, 255f, 1, 0)]
        public override int StiltsNOGrayValue
        {
            get; set;
        }
        #endregion

        #region PADCHECK

        [Category("10.PADCHECK"), DefaultValue(PADMethodEnum.NONE)]
        [Description("是否检查PAD溢胶模式")]
        [DisplayName("0.检查溢胶模式")]
        [TypeConverter(typeof(JzEnumConverter))]
        public override PADMethodEnum PADMethod
        {
            get; set;
        }

        [Category("10.PADCHECK"), DefaultValue(0.1)]
        [Description("与模板的长度比较的比例 大于此值NG 反之OK")]
        [DisplayName("1.长度比例")]
        [TypeConverter(typeof(NumericUpDownTypeConverter))]
        [Editor(typeof(NumericUpDownTypeEditor), typeof(UITypeEditor)), MinMax(0f, 100f, 1, 2)]
        public override double PADOWidthRatio
        {
            get; set;
        }
        [Category("10.PADCHECK"), DefaultValue(0.1)]
        [Description("与模板的宽度比较的比例 大于此值NG 反之OK")]
        [DisplayName("2.宽度比例")]
        [TypeConverter(typeof(NumericUpDownTypeConverter))]
        [Editor(typeof(NumericUpDownTypeEditor), typeof(UITypeEditor)), MinMax(0f, 100f, 1, 2)]
        public override double PADOHeightRatio
        {
            get; set;
        }
        [Category("10.PADCHECK"), DefaultValue(0.1)]
        [Description("与模板的面积比较的比例 大于此值NG 反之OK")]
        [DisplayName("3.面积比例")]
        [TypeConverter(typeof(NumericUpDownTypeConverter))]
        [Editor(typeof(NumericUpDownTypeEditor), typeof(UITypeEditor)), MinMax(0f, 100f, 1, 2)]
        public override double PADOAreaRatio
        {
            get; set;
        }

        [Category("10.PADCHECK"), DefaultValue(PADThresholdEnum.Threshold)]
        [Description("检查PAD图像模式")]
        [DisplayName("4.图像模式")]
        [TypeConverter(typeof(JzEnumConverter))]
        public override PADThresholdEnum PADThresholdMode
        {
            get; set;
        }
        [Category("10.PADCHECK"), DefaultValue(true)]
        [Description("")]
        [DisplayName("4A.找芯片底色白色")]
        //[TypeConverter(typeof(NumericUpDownTypeConverter))]
        //[Editor(typeof(NumericUpDownTypeEditor), typeof(UITypeEditor)), MinMax(0f, 1000000f, 1, 2)]
        public override bool ChipFindWhite
        {
            get; set;
        }
        [Category("10.PADCHECK"), DefaultValue(128)]
        [Description("即 寻找中心位置的灰阶阈值")]
        [DisplayName("4.中心灰阶值")]
        [TypeConverter(typeof(NumericUpDownTypeConverter))]
        [Editor(typeof(NumericUpDownTypeEditor), typeof(UITypeEditor)), MinMax(0f, 255, 1, 0)]
        public override int PADGrayThreshold
        {
            get; set;
        }
        [Category("10.PADCHECK"), DefaultValue(128)]
        [Description("即 寻找中心位置中溢胶的灰阶阈值")]
        [DisplayName("5.中心溢胶灰阶值")]
        [TypeConverter(typeof(NumericUpDownTypeConverter))]
        [Editor(typeof(NumericUpDownTypeEditor), typeof(UITypeEditor)), MinMax(0f, 255, 1, 0)]
        public override int PADBlobGrayThreshold
        {
            get; set;
        }
        [Category("10.PADCHECK"), DefaultValue(true)]
        [Description("")]
        [DisplayName("5B.找银胶底色白色")]
        //[TypeConverter(typeof(NumericUpDownTypeConverter))]
        //[Editor(typeof(NumericUpDownTypeEditor), typeof(UITypeEditor)), MinMax(0f, 1000000f, 1, 2)]
        public override bool GLEFindWhite
        {
            get; set;
        }
        [Category("10.PADCHECK"), DefaultValue(100)]
        [Description("即 寻找中心位置中银胶的灰阶阈值")]
        [DisplayName("5A.银胶灰阶值")]
        [TypeConverter(typeof(NumericUpDownTypeConverter))]
        [Editor(typeof(NumericUpDownTypeEditor), typeof(UITypeEditor)), MinMax(0f, 255, 1, 0)]
        public override int PADChipInBlobGrayThreshold
        {
            get; set;
        }

        [Category("10.PADCHECK"), DefaultValue(15)]
        [Description("溢胶的面积大小 大于此值NG 反之OK (單位 mil)")]
        [DisplayName("6.检查溢胶面积")]
        [TypeConverter(typeof(NumericUpDownTypeConverter))]
        [Editor(typeof(NumericUpDownTypeEditor), typeof(UITypeEditor)), MinMax(0f, 1000000f, 1, 2)]
        public override double PADCheckDArea
        {
            get; set;
        }
        [Category("10.PADCHECK"), DefaultValue(15)]
        [Description("溢胶的长度 大于此值NG 反之OK (單位 mil)")]
        [DisplayName("7.检查溢胶长度")]
        [TypeConverter(typeof(NumericUpDownTypeConverter))]
        [Editor(typeof(NumericUpDownTypeEditor), typeof(UITypeEditor)), MinMax(0f, 1000000f, 1, 2)]
        public override double PADCheckDWidth
        {
            get; set;
        }
        [Category("10.PADCHECK"), DefaultValue(15)]
        [Description("溢胶的宽度 大于此值NG 反之OK (單位 mil)")]
        [DisplayName("7.检查溢胶宽度")]
        [TypeConverter(typeof(NumericUpDownTypeConverter))]
        [Editor(typeof(NumericUpDownTypeEditor), typeof(UITypeEditor)), MinMax(0f, 1000000f, 1, 2)]
        public override double PADCheckDHeight
        {
            get; set;
        }
        [Category("11.PADCHECK"), DefaultValue(5)]
        [Description("(單位 mil)")]
        [DisplayName("8.X方向")]
        [TypeConverter(typeof(NumericUpDownTypeConverter))]
        [Editor(typeof(NumericUpDownTypeEditor), typeof(UITypeEditor)), MinMax(0f, 1000000f, 1, 2)]
        public override double PADExtendX
        {
            get; set;
        }
        [Category("12.PADCHECK"), DefaultValue(5)]
        [Description("(單位 mil)")]
        [DisplayName("9.Y方向")]
        [TypeConverter(typeof(NumericUpDownTypeConverter))]
        [Editor(typeof(NumericUpDownTypeEditor), typeof(UITypeEditor)), MinMax(0f, 1000000f, 1, 2)]
        public override double PADExtendY
        {
            get; set;
        }

        [Category("11.PADCHECK"), DefaultValue(5)]
        [Description("")]
        [DisplayName("spec.薄膜胶最大值 (單位 mm)")]
        [TypeConverter(typeof(NumericUpDownTypeConverter))]
        [Editor(typeof(NumericUpDownTypeEditor), typeof(UITypeEditor)), MinMax(0f, 1000000f, 1, 2)]
        public override double GlueMax
        {
            get; set;
        }
        [Category("11.PADCHECK"), DefaultValue(5)]
        [Description("")]
        [DisplayName("spec.薄膜胶最小值 (單位 mm)")]
        [TypeConverter(typeof(NumericUpDownTypeConverter))]
        [Editor(typeof(NumericUpDownTypeEditor), typeof(UITypeEditor)), MinMax(0f, 1000000f, 1, 2)]
        public override double GlueMin
        {
            get; set;
        }
        [Category("11.PADCHECK"), DefaultValue(false)]
        [Description("")]
        [DisplayName("A1a.是否检查胶水宽度")]
        //[TypeConverter(typeof(NumericUpDownTypeConverter))]
        //[Editor(typeof(NumericUpDownTypeEditor), typeof(UITypeEditor)), MinMax(0f, 1000000f, 1, 2)]
        public override bool GlueCheck
        {
            get; set;
        }
        [Category("11.PADCHECK"), DefaultValue(true)]
        [Description("")]
        [DisplayName("A1b.芯片水平方向")]
        //[TypeConverter(typeof(NumericUpDownTypeConverter))]
        //[Editor(typeof(NumericUpDownTypeEditor), typeof(UITypeEditor)), MinMax(0f, 1000000f, 1, 2)]
        public override bool ChipDirLevel
        {
            get; set;
        }
        [Category("11.PADCHECK"), DefaultValue(true)]
        [Description("")]
        [DisplayName("A1c.芯片接触锡球检测")]
        //[TypeConverter(typeof(NumericUpDownTypeConverter))]
        //[Editor(typeof(NumericUpDownTypeEditor), typeof(UITypeEditor)), MinMax(0f, 1000000f, 1, 2)]
        public override bool ChipGleCheck
        {
            get; set;
        }
        [Category("11.PADCHECK"), DefaultValue(0)]
        [Description("小于设定的值则为四周无胶PASS，反之NG")]
        [DisplayName("A1d.检测四周无胶的值")]
        [TypeConverter(typeof(NumericUpDownTypeConverter))]
        [Editor(typeof(NumericUpDownTypeEditor), typeof(UITypeEditor)), MinMax(0, 255)]
        public override int FourSideNoGluePassValue
        {
            get; set;
        }
        [Category("10.PADCHECK"), DefaultValue(""), ReadOnly(true)]
        //[Description("大芯片与小芯片处理图像模式")]
        [DisplayName("A4a.检测无芯片方法")]
        [TypeConverter(typeof(JzEnumConverter))]
        public override ChipNoHave ChipNoHaveMode
        {
            get; set;
        }
        [Category("10.PADCHECK"), DefaultValue("")]
        [Description("")]
        [DisplayName("A4a.检测无芯片方法参数")]
        [Editor(typeof(GetNoHaveModePropertyEditor), typeof(UITypeEditor))]
        //[TypeConverter(typeof(NumericUpDownTypeConverter))]
        //[Editor(typeof(NumericUpDownTypeEditor), typeof(UITypeEditor)), MinMax(0f, 1f, 0.1f, 2)]
        public override string ChipNoHaveModeOpString
        {
            get; set;
        }
        [Category("10.PADCHECK"), DefaultValue(ChipNoGlueMethod.NONE)]
        //[Description("大芯片与小芯片处理图像模式")]
        [DisplayName("A4b.检测无胶方法")]
        [TypeConverter(typeof(JzEnumConverter))]
        public override ChipNoGlueMethod ChipNoGlueMode
        {
            get; set;
        }
        [Category("11.PADCHECK"), DefaultValue(0.75)]
        [Description("")]
        [DisplayName("A4c.检测无胶容许值")]
        [TypeConverter(typeof(NumericUpDownTypeConverter))]
        [Editor(typeof(NumericUpDownTypeEditor), typeof(UITypeEditor)), MinMax(0f, 1f, 0.1f, 2)]
        public override double NoGlueThresholdValue
        {
            get; set;
        }
        [Category("10.PADCHECK"), DefaultValue(PADCalModeEnum.BlacktoBlack)]
        [Description("计算宽度模式  有电阻模式即周围有电阻抓点到最外边 反之周围无电阻且抓点停止于最大尺寸")]
        [DisplayName("B.计算宽度模式")]
        [TypeConverter(typeof(JzEnumConverter))]
        public override PADCalModeEnum PADCalMode
        {
            get; set;
        }
        [Category("10.PADCHECK"), DefaultValue(PADCalModeEnum.BlacktoBlack)]
        [Description("大芯片与小芯片处理图像模式")]
        [DisplayName("C.计算宽度图像模式")]
        [TypeConverter(typeof(JzEnumConverter))]
        public override PADChipSize PADChipSizeMode
        {
            get; set;
        }
        [Category("10.PADCHECK"), DefaultValue(""), ReadOnly(true)]
        [Description("")]
        [DisplayName("C1.通用模式模型")]
        //[TypeConverter(typeof(NumericUpDownTypeConverter))]
        //[Editor(typeof(NumericUpDownTypeEditor), typeof(UITypeEditor)), MinMax(0f, 1f, 0.1f, 2)]
        public override PadInspectMethodEnum PadInspectMethod
        {
            get; set;
        }
        [Category("10.PADCHECK"), DefaultValue("")]
        [Description("")]
        [DisplayName("C1.通用模式模型参数")]
        [Editor(typeof(GetPADDataPropertyEditor), typeof(UITypeEditor))]
        //[TypeConverter(typeof(NumericUpDownTypeConverter))]
        //[Editor(typeof(NumericUpDownTypeEditor), typeof(UITypeEditor)), MinMax(0f, 1f, 0.1f, 2)]
        public override string PADINSPECTOPString
        {
            get; set;
        }

        [Category("10.PADCHECK"), DefaultValue(AICategory.Baseline)]
        [Description("在AI模式下选择不同模型以适应芯片的大小")]
        [DisplayName("C1.AI模型")]
        [TypeConverter(typeof(JzEnumConverter))]
        public override AICategory PADAICategory
        {
            get; set;
        }
        [Category("11.PADCHECK"), DefaultValue(5)]
        [Description("")]
        [DisplayName("D1.胶水X方向 (單位 mil)")]
        [TypeConverter(typeof(NumericUpDownTypeConverter))]
        [Editor(typeof(NumericUpDownTypeEditor), typeof(UITypeEditor)), MinMax(0f, 1000000f, 1, 2)]
        public override double CalExtendX
        {
            get; set;
        }
        [Category("12.PADCHECK"), DefaultValue(5)]
        [Description("")]
        [DisplayName("D2.胶水Y方向 (單位 mil)")]
        [TypeConverter(typeof(NumericUpDownTypeConverter))]
        [Editor(typeof(NumericUpDownTypeEditor), typeof(UITypeEditor)), MinMax(0f, 1000000f, 1, 2)]
        public override double CalExtendY
        {
            get; set;
        }

        [Category("11.PADCHECK"), DefaultValue(5)]
        [Description("")]
        [DisplayName("D3.黑边X (單位 mil)")]
        [TypeConverter(typeof(NumericUpDownTypeConverter))]
        [Editor(typeof(NumericUpDownTypeEditor), typeof(UITypeEditor)), MinMax(-1000f, 1000000f, 1, 2)]
        public override double BlackCalExtendX
        {
            get; set;
        }
        [Category("12.PADCHECK"), DefaultValue(5)]
        [Description("")]
        [DisplayName("D4.黑边Y (單位 mil)")]
        [TypeConverter(typeof(NumericUpDownTypeConverter))]
        [Editor(typeof(NumericUpDownTypeEditor), typeof(UITypeEditor)), MinMax(-1000f, 1000000f, 1, 2)]
        public override double BlackCalExtendY
        {
            get; set;
        }
        [Category("11.PADCHECK"), DefaultValue(5)]
        [Description("")]
        [DisplayName("D5.黑边X偏移 (單位 mil)")]
        [TypeConverter(typeof(NumericUpDownTypeConverter))]
        [Editor(typeof(NumericUpDownTypeEditor), typeof(UITypeEditor)), MinMax(-1000f, 1000000f, 1, 2)]
        public override double BlackOffsetX
        {
            get; set;
        }
        [Category("12.PADCHECK"), DefaultValue(5)]
        [Description("")]
        [DisplayName("D6.黑边Y偏移 (單位 mil)")]
        [TypeConverter(typeof(NumericUpDownTypeConverter))]
        [Editor(typeof(NumericUpDownTypeEditor), typeof(UITypeEditor)), MinMax(-1000f, 1000000f, 1, 2)]
        public override double BlackOffsetY
        {
            get; set;
        }
        [Category("12.PADCHECK"), DefaultValue(35)]
        [Description("")]
        [DisplayName("D7.字体大小 (單位 pix)")]
        [TypeConverter(typeof(NumericUpDownTypeConverter))]
        [Editor(typeof(NumericUpDownTypeEditor), typeof(UITypeEditor)), MinMax(-1000f, 1000000f, 1, 2)]
        public override int FontSize
        {
            get; set;
        }
        [Category("12.PADCHECK"), DefaultValue(5)]
        [Description("")]
        [DisplayName("D8.线宽 (單位 pix)")]
        [TypeConverter(typeof(NumericUpDownTypeConverter))]
        [Editor(typeof(NumericUpDownTypeEditor), typeof(UITypeEditor)), MinMax(-1000f, 1000000f, 1, 2)]
        public override int LineWidth
        {
            get; set;
        }
        [Category("10.PADCHECK"), DefaultValue(ChipSlotDir.NONE)]
        [Description("即 用来测试薄膜胶的宽度")]
        [DisplayName("D9.标记槽的方向")]
        [TypeConverter(typeof(JzEnumConverter))]
        public override ChipSlotDir GlueChipSlotDir
        {
            get; set;
        }

        [Category("10.PADCHECK"), DefaultValue(0.33)]
        [Description("调整图像的边界的容许值百分比值")]
        [DisplayName("C0.通用模式容许值")]
        [TypeConverter(typeof(NumericUpDownTypeConverter))]
        [Editor(typeof(NumericUpDownTypeEditor), typeof(UITypeEditor)), MinMax(0f, 5f, 0.1f, 2)]
        public override double BloodFillValueRatio
        {
            get; set;
        }

        [Category("10.PADCHECK"), DefaultValue("")]
        [Description("")]
        [DisplayName("E1.扩展参数")]
        [Editor(typeof(GetExtendPropertyEditor), typeof(UITypeEditor))]
        //[TypeConverter(typeof(NumericUpDownTypeConverter))]
        //[Editor(typeof(NumericUpDownTypeEditor), typeof(UITypeEditor)), MinMax(0f, 1f, 0.1f, 2)]
        public override string PADExtendOPString
        {
            get; set;
        }


        [Category("11.PADCHECK"), DefaultValue(0.6)]
        [Description("")]
        [DisplayName("spec.上最大值 (單位 mm)")]
        [TypeConverter(typeof(NumericUpDownTypeConverter))]
        [Editor(typeof(NumericUpDownTypeEditor), typeof(UITypeEditor)), MinMax(0f, 1000000f, 1, 2)]
        public override double GlueMaxTop
        {
            get; set;
        }
        [Category("11.PADCHECK"), DefaultValue(0.1)]
        [Description("")]
        [DisplayName("spec.上最小值 (單位 mm)")]
        [TypeConverter(typeof(NumericUpDownTypeConverter))]
        [Editor(typeof(NumericUpDownTypeEditor), typeof(UITypeEditor)), MinMax(0f, 1000000f, 1, 2)]
        public override double GlueMinTop
        {
            get; set;
        }

        [Category("11.PADCHECK"), DefaultValue(0.6)]
        [Description("")]
        [DisplayName("spec.下最大值 (單位 mm)")]
        [TypeConverter(typeof(NumericUpDownTypeConverter))]
        [Editor(typeof(NumericUpDownTypeEditor), typeof(UITypeEditor)), MinMax(0f, 1000000f, 1, 2)]
        public override double GlueMaxBottom
        {
            get; set;
        }
        [Category("11.PADCHECK"), DefaultValue(0.1)]
        [Description("")]
        [DisplayName("spec.下最小值 (單位 mm)")]
        [TypeConverter(typeof(NumericUpDownTypeConverter))]
        [Editor(typeof(NumericUpDownTypeEditor), typeof(UITypeEditor)), MinMax(0f, 1000000f, 1, 2)]
        public override double GlueMinBottom
        {
            get; set;
        }

        [Category("11.PADCHECK"), DefaultValue(1.2)]
        [Description("")]
        [DisplayName("spec.左最大值 (單位 mm)")]
        [TypeConverter(typeof(NumericUpDownTypeConverter))]
        [Editor(typeof(NumericUpDownTypeEditor), typeof(UITypeEditor)), MinMax(0f, 1000000f, 1, 2)]
        public override double GlueMaxLeft
        {
            get; set;
        }
        [Category("11.PADCHECK"), DefaultValue(0.1)]
        [Description("")]
        [DisplayName("spec.左最小值 (單位 mm)")]
        [TypeConverter(typeof(NumericUpDownTypeConverter))]
        [Editor(typeof(NumericUpDownTypeEditor), typeof(UITypeEditor)), MinMax(0f, 1000000f, 1, 2)]
        public override double GlueMinLeft
        {
            get; set;
        }

        [Category("11.PADCHECK"), DefaultValue(0.9)]
        [Description("")]
        [DisplayName("spec.右最大值 (單位 mm)")]
        [TypeConverter(typeof(NumericUpDownTypeConverter))]
        [Editor(typeof(NumericUpDownTypeEditor), typeof(UITypeEditor)), MinMax(0f, 1000000f, 1, 2)]
        public override double GlueMaxRight
        {
            get; set;
        }
        [Category("11.PADCHECK"), DefaultValue(0.1)]
        [Description("")]
        [DisplayName("spec.右最小值 (單位 mm)")]
        [TypeConverter(typeof(NumericUpDownTypeConverter))]
        [Editor(typeof(NumericUpDownTypeEditor), typeof(UITypeEditor)), MinMax(0f, 1000000f, 1, 2)]
        public override double GlueMinRight
        {
            get; set;
        }


        [Category("11.PADCHECK"), DefaultValue(0.1)]
        [Description("")]
        [DisplayName("specA.01.银胶长度上限 (單位 mm)")]
        [TypeConverter(typeof(NumericUpDownTypeConverter))]
        [Editor(typeof(NumericUpDownTypeEditor), typeof(UITypeEditor)), MinMax(0f, 1000000f, 1, 2)]
        public override double GleWidthUpper
        {
            get; set;
        }
        [Category("11.PADCHECK"), DefaultValue(0.1)]
        [Description("")]
        [DisplayName("specA.02.银胶长度下限 (單位 mm)")]
        [TypeConverter(typeof(NumericUpDownTypeConverter))]
        [Editor(typeof(NumericUpDownTypeEditor), typeof(UITypeEditor)), MinMax(0f, 1000000f, 1, 2)]
        public override double GleWidthLower
        {
            get; set;
        }

        [Category("11.PADCHECK"), DefaultValue(0.1)]
        [Description("")]
        [DisplayName("specA.03.银胶宽度上限 (單位 mm)")]
        [TypeConverter(typeof(NumericUpDownTypeConverter))]
        [Editor(typeof(NumericUpDownTypeEditor), typeof(UITypeEditor)), MinMax(0f, 1000000f, 1, 2)]
        public override double GleHeightUpper
        {
            get; set;
        }
        [Category("11.PADCHECK"), DefaultValue(0.1)]
        [Description("")]
        [DisplayName("specA.04.银胶宽度下限 (單位 mm)")]
        [TypeConverter(typeof(NumericUpDownTypeConverter))]
        [Editor(typeof(NumericUpDownTypeEditor), typeof(UITypeEditor)), MinMax(0f, 1000000f, 1, 2)]
        public override double GleHeightLower
        {
            get; set;
        }

        [Category("11.PADCHECK"), DefaultValue(0.1)]
        [Description("")]
        [DisplayName("specA.05.银胶面积上限 (單位 mm)")]
        [TypeConverter(typeof(NumericUpDownTypeConverter))]
        [Editor(typeof(NumericUpDownTypeEditor), typeof(UITypeEditor)), MinMax(0f, 1000000f, 1, 2)]
        public override double GleAreaUpper
        {
            get; set;
        }
        [Category("11.PADCHECK"), DefaultValue(0.1)]
        [Description("")]
        [DisplayName("specA.06.银胶面积下限 (單位 mm)")]
        [TypeConverter(typeof(NumericUpDownTypeConverter))]
        [Editor(typeof(NumericUpDownTypeEditor), typeof(UITypeEditor)), MinMax(0f, 1000000f, 1, 2)]
        public override double GleAreaLower
        {
            get; set;
        }

        [Category("11.PADCHECK"), DefaultValue(0.1)]
        [Description("")]
        [DisplayName("specA.07.银胶上下距离偏移量 (單位 mm)")]
        [TypeConverter(typeof(NumericUpDownTypeConverter))]
        [Editor(typeof(NumericUpDownTypeEditor), typeof(UITypeEditor)), MinMax(0f, 1000000f, 1, 2)]
        public override double GlueTopBottomOffset
        {
            get; set;
        }
        [Category("11.PADCHECK"), DefaultValue(0.1)]
        [Description("")]
        [DisplayName("specA.08.银胶左右距离偏移量 (單位 mm)")]
        [TypeConverter(typeof(NumericUpDownTypeConverter))]
        [Editor(typeof(NumericUpDownTypeEditor), typeof(UITypeEditor)), MinMax(0f, 1000000f, 1, 2)]
        public override double GlueLeftRightOffset
        {
            get; set;
        }

        #endregion
    }
}
