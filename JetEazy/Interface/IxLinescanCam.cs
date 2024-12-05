using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JetEazy.Interface
{
    public interface IxLineScanCam : IDisposable
    {
        void Init(bool debug, string inipara);
        bool IsSim();
        bool Open();
        bool Open(string configFile);
        bool Close();
        bool IsGrapImageOK { get; set; }
        bool IsGrapImageComplete { get; set; }

        void SoftTrigger();
        /// <summary>
        /// 获取图像
        /// 大于0 放大倍数 等于0 不变尺寸 小于0缩小倍数
        /// </summary>
        /// <param name="size">大于0 放大倍数 等于0 不变尺寸 小于0缩小倍数</param>
        /// <returns>返回图像</returns>
        System.Drawing.Bitmap GetPageBitmap(int size = 0);

        FreeImageAPI.FreeImageBitmap GetFreeImageBitmap(int size = 0);

        //System.Drawing.Bitmap GetPageBitmap(int size);
        void EncoderReset();
        void ShowSetup();


        IntPtr ImagePbuffer { get; set; }
        int ImageWidth { get; set; }
        int ImageHeight { get; set; }
        int ImageRotate { get; set; }
    }
}
