using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using VM.Core;
using VM.PlatformSDKCS;
using IMVSCnnCodeRecgModuCs;
using ImageSourceModuleCs;
using System.Windows.Forms;
using static ImageSourceModuleCs.ImageSourceParam;
using IMVSCnnCodeRecgModuCCs;

namespace Allinone.ZGa.Mvc.Model.BarcodeModel
{
    public class CodeBuilderClassV0 : IxCodeBuilder
    {
        //VmProcedure vmProcedure = null;
        ProcessInfoList vmProcessInfoList;

        public CodeBuilderClassV0() { }
        public string NameStr
        {
            get { return "LGBCreate"; }
        }
        public bool Init()
        {
            try
            {
                string vmSolutionPath = System.IO.Path.Combine(System.Environment.CurrentDirectory, "erweima.sol");
                // "E:\\00_WorkCodeSpace\\Test\\VersionMasterSDKLiuChen01\\erweima.sol";
                VmSolution.Load(vmSolutionPath);//加载方案
                vmProcessInfoList = VmSolution.Instance.GetAllProcedureList();//获取所有流程列
                                                                                              //加载第一个流程                                                                              
                //vmProcedure = VmSolution.Instance[vmProcessInfoList.astProcessInfo[0].strProcessName] as VmProcedure;
            }
            catch (Exception ex)
            {
                //初始流程异常
                MessageBox.Show($"{ex.Message}{Environment.NewLine}造成这种情况原因:{Environment.NewLine}" +
                    $"1:没有插加密狗;{Environment.NewLine}2:VisionMaster软件在运行;");
                Environment.Exit(0);
            }
            return true;
        }
        public string Run(Bitmap bmpInput)
        {

            VmProcedure vmProcedure = VmSolution.Instance[vmProcessInfoList.astProcessInfo[0].strProcessName] as VmProcedure;
            //Bitmap bitmap = new Bitmap("E:\\00_WorkCodeSpace\\Test\\VersionMasterSDKLiuChen01\\0.png");
            ImageBaseData imageBaseData = BitmapToImageBaseData(bmpInput);
            //加载流程图像源模块
            string moduleName = vmProcessInfoList.astProcessInfo[0].strProcessName + ".图像源1";    //cmbSelectProcedure.Text + ".图像源1";
            ImageSourceModuleTool vImage = (ImageSourceModuleTool)VmSolution.Instance[moduleName];

            ImageSourceParam moduParams = vImage.ModuParams;
            moduParams.ImageSourceType = ImageSourceTypeEnum.SDK; //设置图像源类型为SDK

            vImage.SetImageData(imageBaseData);//设置图像路径
            //vImage.SetImagePath("E:\\00_WorkCodeSpace\\Test\\VersionMasterSDKLiuChen01\\0.png"); //设置图像路径

            //获取要绑定的模块名称
            string moduleNameDLerweima = vmProcessInfoList.astProcessInfo[0].strProcessName + ".DL读码C1";
            IMVSCnnCodeRecgModuCTool vCode = (IMVSCnnCodeRecgModuCTool)VmSolution.Instance[moduleNameDLerweima];
            vCode.Run(); //运行模块
            vmProcedure.Run();//运行流程

            CnnCodeRecgCResult moduResult = vCode.ModuResult; //调用模块方法
            if (moduResult.CodeNum != 0)
            {
                string t = moduResult.CodeStr[0];
                //txtData.Text = t + "";
                return t;
            }
            return string.Empty;
        }
        public void Dispose()
        {
            //释放资源
        }

        #region 图片装换
        private ImageBaseData BitmapToImageBaseData(Bitmap bmpInputImg)
        {
            ImageBaseData imageBaseData = new ImageBaseData();
            System.Drawing.Imaging.PixelFormat bitPixelFormat = bmpInputImg.PixelFormat;
            BitmapData bmData = bmpInputImg.LockBits(new Rectangle(0, 0, bmpInputImg.Width, bmpInputImg.Height), ImageLockMode.ReadOnly, bitPixelFormat);//锁定

            if (bitPixelFormat == System.Drawing.Imaging.PixelFormat.Format8bppIndexed)
            {
                Int32 bitmapDataSize = bmData.Stride * bmData.Height;//bitmap图像缓存长度
                int offset = bmData.Stride - bmData.Width;
                Int32 ImageBaseDataSize = bmData.Width * bmData.Height;
                byte[] _BitImageBufferBytes = new byte[bitmapDataSize];
                byte[] _ImageBaseDataBufferBytes = new byte[ImageBaseDataSize];
                Marshal.Copy(bmData.Scan0, _BitImageBufferBytes, 0, bitmapDataSize);
                int bitmapIndex = 0;
                int ImageBaseDataIndex = 0;
                for (int i = 0; i < bmData.Height; i++)
                {
                    for (int j = 0; j < bmData.Width; j++)
                    {
                        _ImageBaseDataBufferBytes[ImageBaseDataIndex++] = _BitImageBufferBytes[bitmapIndex++];
                    }
                    bitmapIndex += offset;
                }
                imageBaseData = new ImageBaseData(_ImageBaseDataBufferBytes, (uint)ImageBaseDataSize, bmData.Width, bmData.Height, (int)VMPixelFormat.VM_PIXEL_MONO_08);
            }
            else if (bitPixelFormat == System.Drawing.Imaging.PixelFormat.Format24bppRgb)
            {
                Int32 bitmapDataSize = bmData.Stride * bmData.Height;//bitmap图像缓存长度
                int offset = bmData.Stride - bmData.Width * 3;
                Int32 ImageBaseDataSize = bmData.Width * bmData.Height * 3;
                byte[] _BitImageBufferBytes = new byte[bitmapDataSize];
                byte[] _ImageBaseDataBufferBytes = new byte[ImageBaseDataSize];
                Marshal.Copy(bmData.Scan0, _BitImageBufferBytes, 0, bitmapDataSize);
                int bitmapIndex = 0;
                int ImageBaseDataIndex = 0;
                for (int i = 0; i < bmData.Height; i++)
                {
                    for (int j = 0; j < bmData.Width; j++)
                    {
                        _ImageBaseDataBufferBytes[ImageBaseDataIndex++] = _BitImageBufferBytes[bitmapIndex + 2];
                        _ImageBaseDataBufferBytes[ImageBaseDataIndex++] = _BitImageBufferBytes[bitmapIndex + 1];
                        _ImageBaseDataBufferBytes[ImageBaseDataIndex++] = _BitImageBufferBytes[bitmapIndex];
                        bitmapIndex += 3;
                    }
                    bitmapIndex += offset;
                }
                imageBaseData = new ImageBaseData(_ImageBaseDataBufferBytes, (uint)ImageBaseDataSize, bmData.Width, bmData.Height, (int)VMPixelFormat.VM_PIXEL_RGB24_C3);
            }
            bmpInputImg.UnlockBits(bmData);  // 解除锁定
            return imageBaseData;
        }

        #endregion

    }
}
