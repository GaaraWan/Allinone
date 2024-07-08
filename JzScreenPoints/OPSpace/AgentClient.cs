using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Drawing;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using System.Windows.Forms;
using AgentMessageClass;
using JzScreenPoints.Interface;

namespace JetEazy.AH
{
    public class AgentClient : IRpiDriver
    {
        public AgentClient()
        {
        }

        private TcpClient Connect2(String ipAddress, int portNum)
        {
            TcpClient xClient = new TcpClient();
            try
            {
                xClient.Connect(ipAddress, portNum);
            }
            catch (Exception ex)
            {
                return null;
            }

            return xClient;

        }

        //Get Camera Real Time Image to Client User.
        public int GetCameraImage(ref Bitmap xBmp, String ipAddress, int portNum)
        {
            int nRetVal = 0;
            using (TcpClient xClient = Connect2(ipAddress, portNum))
            {
                if (xClient == null)
                {
                    return -2; //Connection Error
                }

                if (xClient.Connected)
                {
                    NetworkStream xStrm = xClient.GetStream();

                    SendObj(xStrm, eAngntOperation.eAngntOperation_GetCameraImage);

                    if ((eAngntOperation)ReceiveObj(xStrm) == eAngntOperation.eAngntOperation_ReadyToSend)//Hand-Shake Message
                    {
                        xBmp = (Bitmap)Bitmap.FromStream(xStrm);
                    }
                    else
                    {
                        nRetVal = -1; //Hand-Shake Error
                    }

                    xStrm.Close();
                }
                else
                {
                    nRetVal = -2; //Connection Error
                }
            }

            return nRetVal;
        }

        //Set The Projector Output Image
        public int SetImage(Bitmap xBmp, String ipAddress, int portNum)
        {
            int nRetVal = 0;
            using (TcpClient xClient = Connect2(ipAddress, portNum))
            {
                if (xClient == null)
                {
                    return -2;
                }

                if (xClient.Connected)
                {
                    NetworkStream xStrm = xClient.GetStream();

                    SendObj(xStrm, eAngntOperation.eAngntOperation_SetImage);

                    if ((eAngntOperation)ReceiveObj(xStrm) == eAngntOperation.eAngntOperation_ReadyToReceive)//Hand-Shake Message
                    {
                        SendImage(xStrm, xBmp);
                    }
                    else
                    {
                        nRetVal = -1; //Hand-Shake Error
                    }

                    xStrm.Close();
                }
                else
                {
                    nRetVal = -2; //Connection Error
                }
            }

            return nRetVal;

        }

        //Set The Projector On/Off
        public int SetPower(bool isON, String ipAddress, int portNum)
        {
            int nRetVal = 0;
            using (TcpClient xClient = Connect2(ipAddress, portNum))
            {
                if (xClient == null)
                {
                    return -2;
                }

                if (xClient.Connected)
                {
                    NetworkStream xStrm = xClient.GetStream();

                    SendObj(xStrm, eAngntOperation.eAngntOperation_SetPower);

                    if ((eAngntOperation)ReceiveObj(xStrm) == eAngntOperation.eAngntOperation_ReadyToReceive)//Hand-Shake Message
                    {
                        SendObj(xStrm, isON);
                    }
                    else
                    {
                        nRetVal = -1; //Hand-Shake Error
                    }

                    xStrm.Close();
                }
                else
                {
                    nRetVal = -2; //Connection Error
                }
            }

            return nRetVal;
        }

        //Get Projector Resolution Data
        public int GetResolution(ref Size size, String ipAddress, int portNum)
        {
            int nRetVal = 0;
            using (TcpClient xClient = Connect2(ipAddress, portNum))
            {
                if (xClient == null)
                {
                    return -2;
                }

                if (xClient.Connected)
                {
                    NetworkStream xStrm = xClient.GetStream();

                    SendObj(xStrm, eAngntOperation.eAngntOperation_GetResolution);

                    if ((eAngntOperation)ReceiveObj(xStrm) == eAngntOperation.eAngntOperation_ReadyToSend)//Hand-Shake Message
                    {
                        //size = (Size)ReceiveObj(xStrm);

                        size.Width = (int)ReceiveObj(xStrm);
                        size.Height = (int)ReceiveObj(xStrm);
                    }
                    else
                    {
                        nRetVal = -1; //Hand-Shake Error
                    }

                    xStrm.Close();
                }
                else
                {
                    nRetVal = -2; //Connection Error
                }
            }

            return nRetVal;
        }

        //Save Calibration File and Calibration Bitmap To Agent
        public int SaveCalibration(string fileScrip, Bitmap xBmp, String ipAddress, int portNum)
        {
            int nRetVal = 0;
            using (TcpClient xClient = Connect2(ipAddress, portNum))
            {
                if (xClient == null)
                {
                    return -2;
                }

                if (xClient.Connected)
                {
                    NetworkStream xStrm = xClient.GetStream();

                    SendObj(xStrm, eAngntOperation.eAngntOperation_SaveCalibration);

                    if ((eAngntOperation)ReceiveObj(xStrm) == eAngntOperation.eAngntOperation_ReadyToReceive)//Hand-Shake Message
                    {
                        AgentCalibrationInfo xCaliInfo = new AgentCalibrationInfo();
                        xCaliInfo.m_xCalibration = xBmp;
                        xCaliInfo.m_strShapedata = fileScrip;
                        SendObj(xStrm, xCaliInfo);
                    }
                    else
                    {
                        nRetVal = -1; //Hand-Shake Error
                    }

                    xStrm.Close();
                }
                else
                {
                    nRetVal = -2; //Connection Error
                }
            }

            return nRetVal;
        }

        //Get Calibration File and Calibration Bitmap From Agent
        public int GetCalibration(ref string fileScrip, ref Bitmap xBmp, String ipAddress, int portNum)
        {
            int nRetVal = 0;
            fileScrip = null;
            xBmp = null;
            using (TcpClient xClient = Connect2(ipAddress, portNum))
            {
                if (xClient == null)
                {
                    return -2;
                }

                if (xClient.Connected)
                {
                    NetworkStream xStrm = xClient.GetStream();

                    SendObj(xStrm, eAngntOperation.eAngntOperation_GetCalibration);

                    if ((eAngntOperation)ReceiveObj(xStrm) == eAngntOperation.eAngntOperation_ReadyToSend)//Hand-Shake Message
                    {
                        AgentCalibrationInfo xCaliInfo = (AgentCalibrationInfo)ReceiveObj(xStrm);
                        xBmp = xCaliInfo.m_xCalibration;
                        fileScrip = xCaliInfo.m_strShapedata;
                    }
                    else
                    {
                        nRetVal = -1; //Hand-Shake Error
                    }

                    xStrm.Close();
                }
                else
                {
                    nRetVal = -2; //Connection Error
                }
            }

            return nRetVal;

        }

        //Save bmporg, bmpmask and shapedata to No. no slot
        public int SaveSetup(int nNo, Bitmap xOrg, Bitmap xMask,string Shapedata, String ipAddress, int portNum)
        {
            int nRetVal = 0;
            using (TcpClient xClient = Connect2(ipAddress, portNum))
            {
                if (xClient == null)
                {
                    return -2;
                }

                if (xClient.Connected)
                {
                    NetworkStream xStrm = xClient.GetStream();

                    SendObj(xStrm, eAngntOperation.eAngntOperation_SaveSetup);

                    if ((eAngntOperation)ReceiveObj(xStrm) == eAngntOperation.eAngntOperation_ReadyToReceive)//Hand-Shake Message
                    {
                        AgentSetupInfo xPackedObj = new AgentSetupInfo();
                        xPackedObj.m_nNumber      = nNo;
                        xPackedObj.m_strShapedata = Shapedata;
                        xPackedObj.m_xMask        = xMask;
                        xPackedObj.m_xOrg         = xOrg;

                        SendObj(xStrm, xPackedObj);
                    }
                    else
                    {
                        nRetVal = -1; //Hand-Shake Error
                    }

                    xStrm.Close();
                }
                else
                {
                    nRetVal = -2; //Connection Error
                }
            }

            return nRetVal;
        }

        //Get No. no data to bmporg and shapedata
        public int GetSetup(int no, ref Bitmap bmporg, ref string shapedata, String ipAddress, int portNum)
        {
            int nRetVal = 0;
            using (TcpClient xClient = Connect2(ipAddress, portNum))
            {
                if (xClient == null)
                {
                    return -2;
                }

                if (xClient.Connected)
                {
                    NetworkStream xStrm = xClient.GetStream();

                    SendObj(xStrm, eAngntOperation.eAngntOperation_GetSetup);
                    SendObj(xStrm, no);

                    if ((eAngntOperation)ReceiveObj(xStrm) == eAngntOperation.eAngntOperation_ReadyToSend)//Hand-Shake Message
                    {
                        AgentSetupInfo xPackedObj = (AgentSetupInfo)ReceiveObj(xStrm);
                        shapedata = xPackedObj.m_strShapedata;
                        bmporg = xPackedObj.m_xOrg;
                    }
                    else
                    {
                        nRetVal = -1; //Hand-Shake Error
                    }

                    xStrm.Close();
                }
                else
                {
                    nRetVal = -2; //Connection Error
                }
            }

            return nRetVal;
        }

        //Get No. no array with data in it
        public int GetSetup(ref int[] no, String ipAddress, int portNum)
        {
            int nRetVal = 0;
            no = null;
            using (TcpClient xClient = Connect2(ipAddress, portNum))
            {
                if (xClient == null)
                {
                    return -2;
                }

                if (xClient.Connected)
                {
                    NetworkStream xStrm = xClient.GetStream();

                    SendObj(xStrm, eAngntOperation.eAngntOperation_GetSetupNo);

                    if ((eAngntOperation)ReceiveObj(xStrm) == eAngntOperation.eAngntOperation_ReadyToSend)//Hand-Shake Message
                    {
                        string[] strNo = (string[])ReceiveObj(xStrm);

                        if (strNo.Length > 0)
                        {
                            no = new int[strNo.Length];
                            for (int x = 0; x < strNo.Length; x++)
                            {
                                no[x] = System.Convert.ToInt32(strNo[x].Replace(".agbin", string.Empty));
                            }
                        }
                        else
                        {
                            nRetVal = -2;
                        }
                    }
                    else
                    {
                        nRetVal = -1; //Hand-Shake Error
                    }

                    xStrm.Close();
                }
                else
                {
                    nRetVal = -2; //Connection Error
                }
            }

            return nRetVal;
        }

        //Get No. no array with data in it
        public int GetSetupEx(ref string[] strno, String ipAddress, int portNum)
        {
            int nRetVal = 0;
            strno = null;
            using (TcpClient xClient = Connect2(ipAddress, portNum))
            {
                if (xClient == null)
                {
                    return -2;
                }

                if (xClient.Connected)
                {
                    NetworkStream xStrm = xClient.GetStream();

                    SendObj(xStrm, eAngntOperation.eAngntOperation_GetSetupNo);

                    if ((eAngntOperation)ReceiveObj(xStrm) == eAngntOperation.eAngntOperation_ReadyToSend)//Hand-Shake Message
                    {
                        string[] strNo = (string[])ReceiveObj(xStrm);

                        if (strNo.Length > 0)
                        {
                            strno = new string[strNo.Length];
                            for (int x = 0; x < strNo.Length; x++)
                            {
                                strno[x] = strNo[x].Replace(".agbin", string.Empty);
                            }
                        }
                        else
                        {
                            nRetVal = -2;
                        }
                    }
                    else
                    {
                        nRetVal = -1; //Hand-Shake Error
                    }

                    xStrm.Close();
                }
                else
                {
                    nRetVal = -2; //Connection Error
                }
            }

            return nRetVal;
        }

        public int ShowSetup(int no, String ipAddress, int portNum)
        {
            int nRetVal = 0;
            using (TcpClient xClient = Connect2(ipAddress, portNum))
            {
                if (xClient == null)
                    return -2; //Connection Error

                if (xClient.Connected)
                {
                    NetworkStream xStrm = xClient.GetStream();
                    SendObj(xStrm, eAngntOperation.eAngntOperation_ShowSetup);
                    SendObj(xStrm, no);
                    if ((eAngntOperation)ReceiveObj(xStrm) == eAngntOperation.eAngntOperation_ReadyToSend)//Hand-Shake Message
                    {
                        //do nothing
                    }
                    else //if((eAngntOperation)ReceiveObj(xStrm) == eAngntOperation.eAngntOperation_FileNotFound)
                    {
                        nRetVal = -3; //File Not Found Error
                    }

                    xStrm.Close();
                }
                else
                {
                    nRetVal = -2; //Connection Error
                }
            }

            return nRetVal;
        }

        public int DeleteSetup(int no, String ipAddress, int portNum)
        {
            int nRetVal = 0;
            using (TcpClient xClient = Connect2(ipAddress, portNum))
            {
                if (xClient == null)
                    return -2; //Connection Error

                if (xClient.Connected)
                {
                    NetworkStream xStrm = xClient.GetStream();
                    SendObj(xStrm, eAngntOperation.eAngntOperation_Del_Setup_No);
                    SendObj(xStrm, no);
                    if ((eAngntOperation)ReceiveObj(xStrm) == eAngntOperation.eAngntOperation_ReadyToSend)//Hand-Shake Message
                    {
                        //do nothing
                    }
                    else //if((eAngntOperation)ReceiveObj(xStrm) == eAngntOperation.eAngntOperation_FileNotFound)
                    {
                        nRetVal = -3; //File Not Found Error
                    }

                    xStrm.Close();
                }
                else
                {
                    nRetVal = -2; //Connection Error
                }
            }

            return nRetVal;
        }

        public int AutoCalibration(PointF[,] _screenArray, PointF[,] _realArray ,String ipAddress, int portNum)
        {
            int nRetVal = 0;
            using (TcpClient xClient = Connect2(ipAddress, portNum))
            {
                if (xClient == null)
                    return -2; //Connection Error

                if (xClient.Connected)
                {
                    NetworkStream xStrm = xClient.GetStream();
                    SendObj(xStrm, eAngntOperation.eAngntOperation_AutoCalibration);
                    SendObj(xStrm, _screenArray);
                    SendObj(xStrm, _realArray);

                    if ((eAngntOperation)ReceiveObj(xStrm) == eAngntOperation.eAngntOperation_ReadyToSend)//Hand-Shake Message
                    {
                        //do nothing
                    }
                    else //if((eAngntOperation)ReceiveObj(xStrm) == eAngntOperation.eAngntOperation_FileNotFound)
                    {
                        nRetVal = -3; //File Not Found Error
                    }

                    xStrm.Close();
                }
                else
                {
                    nRetVal = -2; //Connection Error
                }
            }

            return nRetVal;
        }

        public int ViewToWorld(List<PointF> ptfviews,ref List<PointF> ptfworlds, String ipAddress, int portNum)
        {
            int nRetVal = 0;
            using (TcpClient xClient = Connect2(ipAddress, portNum))
            {
                if (xClient == null)
                    return -2; //Connection Error

                if (xClient.Connected)
                {
                    NetworkStream xStrm = xClient.GetStream();
                    SendObj(xStrm, eAngntOperation.eAngntOperation_Cali_ViewToWorld);
                    SendObj(xStrm, ptfviews);
                    if ((eAngntOperation)ReceiveObj(xStrm) == eAngntOperation.eAngntOperation_ReadyToSend)//Hand-Shake Message
                    {
                        //do nothing
                        ptfworlds = (List<PointF>)ReceiveObj(xStrm);
                    }
                    else //if((eAngntOperation)ReceiveObj(xStrm) == eAngntOperation.eAngntOperation_FileNotFound)
                    {
                        nRetVal = -3; //File Not Found Error
                    }

                    xStrm.Close();
                }
                else
                {
                    nRetVal = -2; //Connection Error
                }
            }

            return nRetVal;
        }

        public int DrawMyPaints(List<Point> ptlist)
        {
            //return 0;// SetAllinoneList(ptlist, "192.168.10.2", 5555);
            return SetAllinoneList(ptlist, "192.168.110.3", 5555);
        }
        public int DrawMyPaintRectS(List<Rectangle> m_rectlist)
        {
            //m_Rect_All.Clear();
            //foreach (Rectangle rect in m_rectlist)
            //{
            //    m_Rect_All.Add(rect);
            //}
            //FillDisplay();
            return 0;
        }
        public int SetAllinoneList(List<Point> ptlist, String ipAddress, int portNum)
        {
            int nRetVal = 0;
            using (TcpClient xClient = Connect2(ipAddress, portNum))
            {
                if (xClient == null)
                    return -2; //Connection Error

                if (xClient.Connected)
                {
                    NetworkStream xStrm = xClient.GetStream();
                    SendObj(xStrm, eAngntOperation.eAngntOperation_Cali_ViewToWorld);
                    SendObj(xStrm, ptlist);
                    if ((eAngntOperation)ReceiveObj(xStrm) == eAngntOperation.eAngntOperation_ReadyToSend)//Hand-Shake Message
                    {
                        //do nothing

                    }
                    else //if((eAngntOperation)ReceiveObj(xStrm) == eAngntOperation.eAngntOperation_FileNotFound)
                    {
                        nRetVal = -3; //File Not Found Error
                    }

                    xStrm.Close();
                }
                else
                {
                    nRetVal = -2; //Connection Error
                }
            }

            return nRetVal;
        }

        //Private function
        private void SendObj(NetworkStream sWriter, object xObj)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                IFormatter formatter = new BinaryFormatter();
                formatter.Serialize(ms, xObj);
                byte[] buffer = ms.ToArray();
                sWriter.Write(buffer, 0, (int)buffer.Length);
                ms.Close();
            }
        }

        private object ReceiveObj(NetworkStream sReader)
        {
            IFormatter formatter = new BinaryFormatter();
            return formatter.Deserialize(sReader); //cast the deserialized object 
        }

        private void SendImage(NetworkStream sWriter, Bitmap xBmp)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                xBmp.Save(ms, System.Drawing.Imaging.ImageFormat.Bmp);
                byte[] imageBuffer = ms.GetBuffer();
                sWriter.Write(imageBuffer, 0, (int)ms.Length);
                ms.Close();
            }
        }
    }
}
