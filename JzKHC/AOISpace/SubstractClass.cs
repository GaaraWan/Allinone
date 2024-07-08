using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Windows.Forms;

using JetEazy.BasicSpace;

namespace JzKHC.AOISpace
{
    class SubstractClass
    {
        bool IsDebug
        {
            get
            {
                return false;
            }
        }
        protected JzToolsClass JzTools = new JzToolsClass();

        public Bitmap bmpResult = new Bitmap(1, 1);
        public SubstractClass()
        {

        }
        public void GetSubResult(Bitmap bmpForegroud, Bitmap bmpBackgroud, int MinThreahold,bool IsBlackAndWhite)
        {
            int RGrade = 0;
            int BGrade = 0;

            bmpResult.Dispose();
            bmpResult = (Bitmap)bmpForegroud.Clone();

            Rectangle rectbmp = JzTools.SimpleRect(bmpResult.Size);

            BitmapData bmpRData = bmpResult.LockBits(rectbmp, ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
            BitmapData bmpBData = bmpBackgroud.LockBits(rectbmp, ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);

            IntPtr RScan0 = bmpRData.Scan0;
            IntPtr BScan0 = bmpBData.Scan0;

            try
            {
                unsafe
                {
                    byte* Rscan0 = (byte*)(void*)RScan0;
                    byte* RpucPtr;
                    byte* RpucStart;

                    byte* Bscan0 = (byte*)(void*)BScan0;
                    byte* BpucPtr;
                    byte* BpucStart;

                    int xmin = rectbmp.X;
                    int ymin = rectbmp.Y;

                    int xmax = xmin + rectbmp.Width;
                    int ymax = ymin + rectbmp.Height;

                    int x = xmin;
                    int y = ymin;

                    int iRStride = bmpRData.Stride;
                    int iBStride = bmpBData.Stride;

                    y = ymin;
                    RpucStart = Rscan0 + ((x - xmin) << 2) + (iRStride * (y - ymin));
                    BpucStart = Bscan0 + ((x - xmin) << 2) + (iBStride * (y - ymin));

                    while (y < ymax)
                    {
                        x = xmin;

                        RpucPtr = RpucStart;
                        BpucPtr = BpucStart;

                        while (x < xmax)
                        {
                            RGrade = JzTools.GrayscaleInt(RpucPtr[2], RpucPtr[1], RpucPtr[0]);
                            BGrade = JzTools.GrayscaleInt(BpucPtr[2], BpucPtr[1], BpucPtr[0]);

                            if (MinThreahold > Math.Abs(RGrade - BGrade))
                            {
                                *((uint*)RpucPtr) = 0xFF000000;
                            }
                            else
                            {
                                if(IsBlackAndWhite)
                                    *((uint*)RpucPtr) = 0xFFFFFFFF;
                            }
                            RpucPtr += 4;
                            BpucPtr += 4;
                            x++;
                        }
                        RpucStart += iRStride;
                        BpucStart += iBStride;
                        y++;
                    }
                    bmpResult.UnlockBits(bmpRData);
                    bmpBackgroud.UnlockBits(bmpBData);
                }
            }
            catch (Exception e)
            {
                bmpResult.UnlockBits(bmpRData);
                bmpBackgroud.UnlockBits(bmpBData);

                if (IsDebug)
                    MessageBox.Show("Error :" + e.ToString());
            }
        }
        public void SetMask(Bitmap bmpOrigin, Bitmap bmpThreshed, Rectangle Rect,Point BiasPoint)
        {
            Rectangle rectbmp = Rect;
            Rectangle rectThreshed = new Rectangle(0, 0, bmpThreshed.Width, bmpThreshed.Height);
            
            BitmapData bmpOData = bmpOrigin.LockBits(rectbmp, ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
            BitmapData bmpTData = bmpThreshed.LockBits(rectThreshed, ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);

            IntPtr OScan0 = bmpOData.Scan0;
            IntPtr TScan0 = bmpTData.Scan0;

            try
            {
                unsafe
                {
                    byte* Oscan0 = (byte*)(void*)OScan0;
                    byte* OpucPtr;
                    byte* OpucStart;

                    byte* Tscan0 = (byte*)(void*)TScan0;
                    byte* TpucPtr;
                    byte* TpucStart;

                    int xmin = rectbmp.X;
                    int ymin = rectbmp.Y;

                    int xmax = xmin + rectbmp.Width;
                    int ymax = ymin + rectbmp.Height;

                    int x = xmin;
                    int y = ymin;

                    int iOStride = bmpOData.Stride;
                    int iTStride = bmpTData.Stride;

                    y = ymin;
                    OpucStart = Oscan0 + ((x - xmin) << 2) + (iOStride * (y - ymin));
                    TpucStart = Tscan0 + ((x - xmin - BiasPoint.X) << 2) + (iTStride * (y - ymin - BiasPoint.Y));

                    while (y < ymax)
                    {
                        x = xmin;

                        OpucPtr = OpucStart;
                        TpucPtr = TpucStart;

                        while (x < xmax)
                        {
                            if ((TpucPtr[2]) == 0xFF)
                            {
                                *((uint*)OpucPtr) = *((uint*)TpucPtr);
                            }

                            OpucPtr += 4;
                            TpucPtr += 4;
                            x++;
                        }
                        OpucStart += iOStride;
                        TpucStart += iTStride;
                        y++;
                    }
                    bmpOrigin.UnlockBits(bmpOData);
                    bmpThreshed.UnlockBits(bmpTData);
                }
            }
            catch (Exception e)
            {
                bmpOrigin.UnlockBits(bmpOData);
                bmpThreshed.UnlockBits(bmpTData);

                if (IsDebug)
                    MessageBox.Show("Error :" + e.ToString());
            }
        }
    }
}
