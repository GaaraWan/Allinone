using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Allinone.BasicSpace
{
    public class JzSliderItemClass
    {
        public JzSliderItemClass() { }
        public int IntOperate { get; set; } = 0;
        public int IntResult { get; set; } = -1;
        public string StrMessage { get; set; } = string.Empty;
        public string AnalyzeNameStr { get; set; } = string.Empty;
        public int IntStepIndex { get; set; } = 0;
        public string AnalyzeOpeateStr { get; set; } = string.Empty;
        public string Mapping2dStr { get; set; } = string.Empty;
        public string Read2dStr { get; set; } = string.Empty;
        public string Show2dMessage { get; set; } = string.Empty;
        public bool AnalyzeBypass { get; set; } = false;
        public void Reset()
        {
            IntOperate = 0;
            IntResult = -1;
            StrMessage = string.Empty;
            //AnalyzeNameStr = string.Empty;
            IntStepIndex = 0;
            Read2dStr = string.Empty;
            AnalyzeBypass = false;
        }
        public void ResetMapping2d()
        {
            Mapping2dStr = string.Empty;
        }
        public bool CheckBarcode()
        {
            if (string.IsNullOrEmpty(Mapping2dStr) && string.IsNullOrEmpty(Read2dStr))
                return false;
            if (Mapping2dStr != Read2dStr)
                return false;
            return true;
        }
        public void Clone(JzSliderItemClass jzSliderItem)
        {
            IntOperate = jzSliderItem.IntOperate;
            IntResult = jzSliderItem.IntResult;
            StrMessage = jzSliderItem.StrMessage;
            AnalyzeNameStr = jzSliderItem.AnalyzeNameStr;
            IntStepIndex = jzSliderItem.IntStepIndex;
            AnalyzeOpeateStr = jzSliderItem.AnalyzeOpeateStr;
            Read2dStr = jzSliderItem.Read2dStr;
            Show2dMessage = jzSliderItem.Show2dMessage;
        }


    }
}
