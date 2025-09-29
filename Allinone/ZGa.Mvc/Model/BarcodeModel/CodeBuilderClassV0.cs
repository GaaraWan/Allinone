using ImageSourceModuleCs;
using IMVSCnnCodeRecgModuCCs;
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
using static ImageSourceModuleCs.ImageSourceParam;

namespace Allinone.ZGa.Mvc.Model.BarcodeModel
{
    public class CodeBuilderClassV0 : IxCodeBuilder
    {
        #region 字段
        //sol路径
        string vmSolutionPath = string.Empty;
        //第一个流程名称
        string firstFlowName = string.Empty;
        //流程对象
        VmProcedure vmProcedure;
        #endregion

        public CodeBuilderClassV0() { }
        public string NameStr
        {
            get { return "LGBCreate"; }
        }
        public bool Init()
        {
            vmSolutionPath = "D:\\JETEAZY\\ALLINONE-MAIN_X6\\WORK\\erweima.sol";
            VmSolution.Load(vmSolutionPath);//加载方案
            ProcessInfoList vmProcessInfoList = VmSolution.Instance.GetAllProcedureList();//获取所有流程列
            firstFlowName = vmProcessInfoList.astProcessInfo[0].strProcessName; //获取第一个流程名称
            //加载第一个流程                                                                              
            vmProcedure = VmSolution.Instance[firstFlowName] as VmProcedure;

            return true;
        }
        public string Run(Bitmap bmpInput)
        {
            //bmpInput.Save("E:\\0924\\3.png");
            if (bmpInput == null)
            {
                return string.Empty;
            }
            //Bitmap bitmap = new Bitmap("E:\\00_WorkCodeSpace\\Test\\VersionMasterSDKLiuChen01\\0.png");
            ImageBaseData imageBaseData = BitmapToImageBaseData(bmpInput);

            //加载流程图像源模块
            string moduleName = firstFlowName + ".图像源1";    //cmbSelectProcedure.Text + ".图像源1";
            ImageSourceModuleTool vImage = (ImageSourceModuleTool)VmSolution.Instance[moduleName];

            ImageSourceParam moduParams = vImage.ModuParams;
            moduParams.ImageSourceType = ImageSourceTypeEnum.SDK; //设置图像源类型为SDK

            vImage.SetImageData(imageBaseData);//设置图像路径
            //vImage.SetImagePath("E:\\00_WorkCodeSpace\\Test\\VersionMasterSDKLiuChen01\\0.png"); //设置图像路径

            //获取要绑定的模块名称
            string moduleNameDLerweima = firstFlowName + ".DL读码C1";
            IMVSCnnCodeRecgModuCTool vCode = (IMVSCnnCodeRecgModuCTool)VmSolution.Instance[moduleNameDLerweima];
            //vCode.Run(); //运行模块
            vmProcedure.Run();//运行流程

            CnnCodeRecgCResult moduResult = vCode.ModuResult; //调用模块方法
            if (moduResult.CodeNum != 0)
            {
                return moduResult.CodeStr[0];
            }

            return string.Empty;
        }

        #region bitmap转换

        /// <summary>
        /// 将Bitmap转换为ImageBaseData
        /// - 8位索引图直接处理
        /// - 24位RGB图先转为8位索引图再处理
        /// </summary>
        public ImageBaseData BitmapToImageBaseData(Bitmap bmpInputImg)
        {
            if (bmpInputImg == null)
                throw new ArgumentNullException(nameof(bmpInputImg), "输入Bitmap不能为空");

            // 处理24位RGB图：先转换为8位索引图
            if (bmpInputImg.PixelFormat == PixelFormat.Format24bppRgb)
            {
                using (Bitmap converted8bpp = Convert24bppTo8bppIndexed(bmpInputImg))
                {
                    return Process8bppIndexedBitmap(converted8bpp);
                }
            }
            // 处理8位索引图
            else if (bmpInputImg.PixelFormat == PixelFormat.Format8bppIndexed)
            {
                return Process8bppIndexedBitmap(bmpInputImg);
            }
            // 不支持的格式
            else
            {
                throw new NotSupportedException($"不支持的像素格式: {bmpInputImg.PixelFormat}，仅支持8位索引图和24位RGB图");
            }
        }

        /// <summary>
        /// 将24位RGB图转换为8位灰度索引图
        /// </summary>
        private Bitmap Convert24bppTo8bppIndexed(Bitmap rgbBitmap)
        {
            int width = rgbBitmap.Width;
            int height = rgbBitmap.Height;

            // 创建8位索引图并设置灰度调色板
            Bitmap indexedBitmap = new Bitmap(width, height, PixelFormat.Format8bppIndexed);
            ColorPalette palette = indexedBitmap.Palette;
            for (int i = 0; i < 256; i++)
            {
                palette.Entries[i] = Color.FromArgb(i, i, i); // 灰度调色板
            }
            indexedBitmap.Palette = palette;

            // 锁定图像数据
            Rectangle rect = new Rectangle(0, 0, width, height);
            BitmapData srcData = rgbBitmap.LockBits(rect, ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);
            BitmapData destData = indexedBitmap.LockBits(rect, ImageLockMode.WriteOnly, PixelFormat.Format8bppIndexed);

            try
            {
                // 源图像数据指针和步长
                IntPtr srcScan0 = srcData.Scan0;
                int srcStride = srcData.Stride;
                int srcPixelSize = 3;

                // 目标图像数据指针和步长
                IntPtr destScan0 = destData.Scan0;
                int destStride = destData.Stride;
                int destPixelSize = 1;

                // 逐行处理像素
                for (int y = 0; y < height; y++)
                {
                    // 计算行地址（处理Stride正负值）
                    IntPtr srcRowPtr = GetRowPtr(srcScan0, srcStride, y, height);
                    IntPtr destRowPtr = GetRowPtr(destScan0, destStride, y, height);

                    // 读取源行数据
                    byte[] srcRow = new byte[Math.Abs(srcStride)];
                    Marshal.Copy(srcRowPtr, srcRow, 0, srcRow.Length);

                    // 计算灰度值并写入目标行
                    byte[] destRow = new byte[Math.Abs(destStride)];
                    for (int x = 0; x < width; x++)
                    {
                        int srcIdx = x * srcPixelSize;
                        byte b = srcRow[srcIdx];
                        byte g = srcRow[srcIdx + 1];
                        byte r = srcRow[srcIdx + 2];

                        // 计算灰度值（ luminance公式 ）
                        byte gray = (byte)(0.299 * r + 0.587 * g + 0.114 * b);
                        destRow[x * destPixelSize] = gray;
                    }

                    // 复制到目标图像
                    Marshal.Copy(destRow, 0, destRowPtr, destRow.Length);
                }

                return indexedBitmap;
            }
            finally
            {
                // 确保释放资源
                rgbBitmap.UnlockBits(srcData);
                indexedBitmap.UnlockBits(destData);
            }
        }

        /// <summary>
        /// 处理8位索引图，转换为ImageBaseData
        /// </summary>
        private ImageBaseData Process8bppIndexedBitmap(Bitmap bmpInputImg)
        {
            BitmapData bmData = null;
            try
            {
                // 锁定图像数据
                Rectangle rect = new Rectangle(0, 0, bmpInputImg.Width, bmpInputImg.Height);
                bmData = bmpInputImg.LockBits(rect, ImageLockMode.ReadOnly, PixelFormat.Format8bppIndexed);

                int bitmapDataSize = bmData.Stride * bmData.Height;
                int offset = bmData.Stride - bmData.Width;
                int imageBaseDataSize = bmData.Width * bmData.Height;

                byte[] bitImageBufferBytes = new byte[bitmapDataSize];
                byte[] imageBaseDataBufferBytes = new byte[imageBaseDataSize];

                // 复制像素数据
                Marshal.Copy(bmData.Scan0, bitImageBufferBytes, 0, bitmapDataSize);

                // 处理数据（去除填充字节）
                int bitmapIndex = 0;
                int imageBaseDataIndex = 0;
                for (int i = 0; i < bmData.Height; i++)
                {
                    for (int j = 0; j < bmData.Width; j++)
                    {
                        imageBaseDataBufferBytes[imageBaseDataIndex++] = bitImageBufferBytes[bitmapIndex++];
                    }
                    bitmapIndex += offset;
                }

                // 创建并返回ImageBaseData
                return new ImageBaseData(
                    imageBaseDataBufferBytes,
                    (uint)imageBaseDataSize,
                    bmData.Width,
                    bmData.Height,
                    (int)VMPixelFormat.VM_PIXEL_MONO_08
                );
            }
            finally
            {
                // 确保解锁
                if (bmData != null)
                {
                    bmpInputImg.UnlockBits(bmData);
                }
            }
        }

        /// <summary>
        /// 计算行数据地址（处理Stride正负值，兼容图像翻转）
        /// </summary>
        private IntPtr GetRowPtr(IntPtr scan0, int stride, int y, int height)
        {
            long rowOffset = stride > 0
                ? y * (long)stride
                : (height - 1 - y) * (long)Math.Abs(stride);
            return new IntPtr(scan0.ToInt64() + rowOffset);
        }

        #endregion

        public void Dispose()
        {
            //释放资源
        }
    }
}
