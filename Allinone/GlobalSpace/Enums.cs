using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace Allinone
{

    public enum FactoryShopfloor
    {
        NONE,
        FOXCONN,
        QSMC,
    }
    public enum PageOPTypeEnum : int
    {
        COUNT = 1,
        AOICOUNT = 1,
        
        P00 = 0,
        P01 = 1,
        P02 = 2,
    }
    public enum MortorName : int
    {
        COUNT = 8,

        A1 = 0,
        A2 = 1,
        A3 = 2,
        A4 = 3,
        A5 = 4,
        A6 = 5,
        A7 = 6,
        A8 = 7,
    }
    public enum AnalyzeTypeEnum : int
    {
        BRANCH = 0,
        LEARNING = 1,
    }
    
    public enum AlignMethodEnum : int
    {
        NONE = -1,

        AUFIND = 0,
        AUMATCH = 1,
    }
    public enum AlignModeEnum : int
    {
        AREA = 0,
        BORDER = 1,
    }

    public enum InspectionMethodEnum :int
    {
        NONE =-1,
        /// <summary>
        /// 绝对值
        /// </summary>
        PIXEL = 0,
        /// <summary>
        /// 蔡式算法
        /// </summary>
        HISTO = 1,
        /// <summary>
        /// 均衡化方式
        /// </summary>
        Equalize = 3,

        /// <summary>
        /// 检查条码
        /// </summary>
        BAR_CHECK = 4,
    }
    public enum Inspection_A_B_Enum : int
    {
        /// <summary>
        /// 直方图差异法
        /// </summary>
        Histogram = 0,
        /// <summary>
        /// AB法
        /// </summary>
        AB = 1,
        /// <summary>
        /// AB 加强法
        /// </summary>
       ABPlus = 2,

       
    }
    public enum MeasureMethodEnum : int
    {
        COUNT = 4,

        NONE = -1,

        BLIND = 0,
        BLOBS = 1,
        MBCHECK = 2,
        COLORCHECK=3,

    }
    public enum OCRMethodEnum : int
    {
        NONE = -1,

        DIRECT = 0,
        MAPPING = 1,
        QRCODE = 2,
        DATAMATRIX = 3,
        CODE39 = 4,
        CODE128 = 5,
        /// <summary>
        /// 检查字体到下边距离
        /// </summary>
        CHICKLINE = 6,
        DATAMATRIXGRADE = 7,
    }
    public enum OCRSETEnum : int
    {
        NONE = -1,

        OCR=0,
    }
    public enum AOIMethodEnum : int
    {
        NONE = -1,

        KBAOI = 0,
    }

    public enum GapMethodEnum : int
    {
        NONE = -1,

        KBGAP = 0,
        LASER = 1,
        //左侧虚拟线
        LL=2,
        //左下虚拟线
        LD =3,
        //右侧虚拟线
        RR =4,
        //右下虚拟线
        RD =5,


        ST=6,
        SN=7,

        //测log
        Logo=8,
        LU=9,
        RU=10,

    }
    /// <summary>
    /// 高跷检查模式
    /// </summary>
    public enum STILTSMethodEnum : int
    {
        NONE = -1,

        STILTS = 0,
    
    }

    /// <summary>
    /// PAD溢胶检查模式
    /// </summary>
    public enum PADMethodEnum : int
    {
        NONE = -1,

        /// <summary>
        /// PAD溢胶检查
        /// </summary>
        [Description("PAD溢胶检查")]
        PADCHECK = 0,

        /// <summary>
        /// 晶片溢胶检查
        /// </summary>
        [Description("晶片溢胶检查")]
        GLUECHECK = 1,

        /// <summary>
        /// 晶片溢胶检查黑边模式
        /// </summary>
        [Description("晶片溢胶检查黑边模式")]
        GLUECHECK_BlackEdge = 2,

    }

    /// <summary>
    /// PAD溢胶图像处理模式
    /// </summary>
    public enum PADThresholdEnum : int
    {
        //NONE = -1,

        /// <summary>
        /// 灰阶模式
        /// </summary>
        [Description("灰阶模式")]
        Threshold = 0,

        /// <summary>
        /// 灰阶模式
        /// </summary>
        [Description("Ostu模式")]
        Ostu_Threshold = 1,

    }
    /// <summary>
    /// PAD溢胶计算宽度模式
    /// </summary>
    public enum PADCalModeEnum : int
    {
        //NONE = -1,

        /// <summary>
        /// 有电阻模式
        /// </summary>
        [Description("有电阻模式")]
        BlacktoBlack = 0,

        /// <summary>
        /// 无电阻模式
        /// </summary>
        [Description("无电阻模式")]
        BlackLast = 1,

    }
    /// <summary>
    /// PAD胶水宽度 图像处理模式 大芯片的图像于小芯片处理不太一样 所以这里区分一下 将来有更好的处理方法 可以 继续添加
    /// </summary>
    public enum PADChipSize : int
    {
        //NONE = -1,

        /// <summary>
        /// 通用模式
        /// </summary>
        [Description("通用模式")]
        CHIP_NORMAL = 0,

        /// <summary>
        /// V1模式
        /// </summary>
        [Description("V1模式"), Browsable(false)]
        CHIP_V1 = 1,

        /// <summary>
        /// V2模式
        /// </summary>
        [Description("V2模式"), Browsable(false)]
        CHIP_V2 = 2,

        /// <summary>
        /// V3模式 canny
        /// </summary>
        [Description("V3模式"), Browsable(false)]
        CHIP_V3 = 3,

        /// <summary>
        /// V5模式 每条边单独处理
        /// </summary>
        [Description("V5模式"), Browsable(false)]
        CHIP_V5 = 4,

        /// <summary>
        /// V6模式 contrast - local - filter blob or - invert - filter blob and 
        /// </summary>
        [Description("V6模式"), Browsable(false)]
        CHIP_V6 = 5,

        /// <summary>
        /// V8模式 AI 预测模式
        /// </summary>
        [Description("AI模式")]
        CHIP_V8 = 6,

    }

    public enum HeightMethodEnum : int
    {
        NONE = -1,

        KBHEIGHT = 0,
    }
    public enum MaskMethodEnum : int
    {
        NONE = -1,

        DIRECT = 0,
        RELATE = 1,

        DIRECTW = 2,
        RELATEW = 3,
    }
    public enum CheckColorMethodEnum : int
    {
        NONE = -1,

        BLUEKEY = 0,
    }
    public enum UselessMethodEnum : int
    {
        NONE = -1,

        BLUEKEY = 0,
    }
    public enum CheckDirtMethodEnum : int
    {
        NONE = -1,

        ALLCHECK = 0,
        OMMITDARK = 1,
        OMMITBRIGHT = 2,
    }
    public enum TestMethodEnum  //是從哪邊來的測試
    {
        BUTTON,
        IO,
        BARCODE,
        QSMCSF,
        MEMTRIGGER,
        CCDTRIGGER,
    }

    public enum OPLevelEnum
    {
        COPY,

        ALB,
        ENV,
        PAGE,
        ANALYZE,
    }
    public enum LearnOperEnum
    {
        TUNE,
        LEARN,
        COMP,

        //作為 GetAnalyze 時的
        THIS,
        PARENT,
        LEARLAST,

    }

    public enum SubOperEnum
    {
        PATTERN,
        MASK,
        OUTPUT,

        CHANGE,

    }
    public enum ColorCheckTypeEnum : int
    {
        COUNT = 6,

        BLUEKEY = 0,

        SILVERSCREW = 1,
        GOLDSCREW = 2,
        GREYSCREW = 3,
        ROSESCREW = 4,

        TP = 5,
    }
    public enum TPColorEnum : int
    {
        NONE = -1,

        SILVER = 0,
        GOLD = 1,
        GREY = 2,
        ROSE = 3,
    }




    public enum DFlyMethodEnum
    {
        RELATEPOS,
        DIRECTPOS,

    }
    public enum ColorCheckMethodEnum : int
    {
        COUNT = 6,

        QSMC100 = 0,
        QSMC1000 = 1,
        QSMCUSB = 2,

        FXCD100 = 3,
        FXCDUSB = 4,
        FXCD1000 = 5,
    }

    public enum UserOptionEnum
    {
        ALL,
        SIDE,
        SELECTED,
    }

    public enum AbsoluteAlignEnum : int
    {
        //COUNT = 3,

        NONE = 0,
        MAIN = 1,
        RELATION = 2,
    }
    public enum BorderTypeEnum : int
    {
        COUNT = 4,

        LEFT = 0,
        TOP = 1,
        RIGHT = 2,
        BOTTOM = 3,
    }

    

}
