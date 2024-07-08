using Allinone.OPSpace;
using ServiceMessageClass;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Allinone.BasicSpace
{
    public class ServiceServerClass
    {

        private TcpListener _server;
        private bool _isRunning;

		public ServiceServerClass(int port)
		{
			////initial server and start
			//_server = new TcpListener(IPAddress.Parse("127.0.0.1"), port); //_server = new TcpListener(IPAddress.Any, port);
			//_server.Start();
			//_isRunning = true;
			//Thread xMainLoop = new Thread(new ThreadStart(LoopClients)); //start to listen
			//xMainLoop.Start();
		}

	}
    public class ServiceClientClass
    {
        //AlbumCollectionClass ALBCollection
        //{
        //    get
        //    {
        //        return Universal.ALBCollection;
        //    }
        //}
        //AlbumClass AlbumWorkNow
        //{
        //    get
        //    {
        //        return ALBCollection.AlbumNow;
        //    }
        //}

        public ServiceClientClass()
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
                    //NetworkStream xStrm = xClient.GetStream();

                    //SendObj(xStrm, eAngntOperation.eAngntOperation_SetImage);

                    //if ((eAngntOperation)ReceiveObj(xStrm) == eAngntOperation.eAngntOperation_ReadyToReceive)//Hand-Shake Message
                    //{
                    //    SendImage(xStrm, xBmp);
                    //}
                    //else
                    //{
                    //    nRetVal = -1; //Hand-Shake Error
                    //}

                    //xStrm.Close();
                }
                else
                {
                    nRetVal = -2; //Connection Error
                }
            }

            return nRetVal;

        }
       
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
                    //NetworkStream xStrm = xClient.GetStream();

                    //SendObj(xStrm, eAngntOperation.eAngntOperation_SaveCalibration);

                    //if ((eAngntOperation)ReceiveObj(xStrm) == eAngntOperation.eAngntOperation_ReadyToReceive)//Hand-Shake Message
                    //{
                    //    AgentCalibrationInfo xCaliInfo = new AgentCalibrationInfo();
                    //    xCaliInfo.m_xCalibration = xBmp;
                    //    xCaliInfo.m_strShapedata = fileScrip;
                    //    SendObj(xStrm, xCaliInfo);
                    //}
                    //else
                    //{
                    //    nRetVal = -1; //Hand-Shake Error
                    //}

                    //xStrm.Close();
                }
                else
                {
                    nRetVal = -2; //Connection Error
                }
            }

            return nRetVal;
        }

        public int PageTrain(SvPageInfo xPageInfo,ref String xWorkTrainStripStr, String ipAddress, int portNum)
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

                    SendObj(xStrm, eServiceOperation.eServiceOperation_PageTrain);

                    if ((eServiceOperation)ReceiveObj(xStrm) == eServiceOperation.eServiceOperation_ReadyToReceive)//Hand-Shake Message
                    {
                        SendObj(xStrm, xPageInfo);
                        if ((eServiceOperation)ReceiveObj(xStrm) == eServiceOperation.eServiceOperation_PageTrainComplete)//Hand-Shake Message
                        {
                            xWorkTrainStripStr = (String)ReceiveObj(xStrm);
                        }
                        else
                        {
                            nRetVal = -3; //Train Not Complete
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
        public int PageRun(Bitmap xbmpInput,ref String xWorkRunStripStr, String ipAddress, int portNum)
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

                    SendObj(xStrm, eServiceOperation.eServiceOperation_PageRun);

                    if ((eServiceOperation)ReceiveObj(xStrm) == eServiceOperation.eServiceOperation_ReadyToReceive)//Hand-Shake Message
                    {
                        SendImage(xStrm, new Bitmap(xbmpInput));
                        if ((eServiceOperation)ReceiveObj(xStrm) == eServiceOperation.eServiceOperation_PageRunComplete)//Hand-Shake Message
                        {
                            xWorkRunStripStr = (String)ReceiveObj(xStrm);
                        }
                        else
                        {
                            nRetVal = -3; //Train Not Complete
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
        public int PageRun(SvPageInfo xPageInfo, ref String xWorkRunStripStr, String ipAddress, int portNum)
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

                    SendObj(xStrm, eServiceOperation.eServiceOperation_PageRun);
                    SendObj(xStrm, xPageInfo);
                    if ((eServiceOperation)ReceiveObj(xStrm) == eServiceOperation.eServiceOperation_ReadyToReceive)//Hand-Shake Message
                    {
                        
                        if ((eServiceOperation)ReceiveObj(xStrm) == eServiceOperation.eServiceOperation_PageRunComplete)//Hand-Shake Message
                        {
                            xWorkRunStripStr = (String)ReceiveObj(xStrm);
                        }
                        else
                        {
                            nRetVal = -3; //Train Not Complete
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
