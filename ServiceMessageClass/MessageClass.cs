using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace ServiceMessageClass
{
    [Serializable]
    public enum eServiceOperation
    {
        eServiceOperation_PageTrain = 0,
        eServiceOperation_PageTrainComplete,
        eServiceOperation_PageRun,
        eServiceOperation_PageRunComplete,
        eServiceOperation_ReadyToSend,
        eServiceOperation_ReadyToReceive,
    }

    [Serializable]
    public class SvPageInfo
    {
        public Bitmap m_Org = null;
        public string m_PassInfoStr = string.Empty;
        public string m_PageStr = string.Empty;
        public string m_AnalyzeStr = string.Empty;

        public bool Save(string strFileName)
        {
            try
            {
                ArrayList xPackage = new ArrayList();
                //xPackage.Add(this.m_xCalibration);
                //xPackage.Add(this.m_strShapedata);

                //create stream obj
                FileStream oFileStream = new FileStream(strFileName + ".svbin", FileMode.Create);

                //create binary format obj 
                BinaryFormatter myBinaryFormatter = new BinaryFormatter();

                //Serialize obj to file and store
                myBinaryFormatter.Serialize(oFileStream, xPackage);
                oFileStream.Flush();
                oFileStream.Close();
                oFileStream.Dispose();
            }
            catch (Exception ex)
            {
                return false;
            }

            return true;
        }

        public bool Load(string strFileName)
        {
            try
            {
                FileStream oFileStream = new FileStream(strFileName, FileMode.Open);
                BinaryFormatter myBinaryFormatter = new BinaryFormatter();
                ArrayList xPackage = (ArrayList)myBinaryFormatter.Deserialize(oFileStream);

                //this.m_xCalibration = (Bitmap)xPackage[0];
                //this.m_strShapedata = (string)xPackage[1];

                oFileStream.Close();
                oFileStream.Dispose();
            }
            catch (Exception ex)
            {
                return false;
            }

            return true;
        }
    }

}
