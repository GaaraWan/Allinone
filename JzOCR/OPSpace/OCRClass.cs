//#define COGNEX
#define AUVISION

using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Data;
using System.Drawing;
using System.IO;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

using JetEazy;
using JetEazy.BasicSpace;
using WorldOfMoveableObjects;
#if(AUVISION)
using AUVision;
#endif
#if(COGNEX)
using Cognex.VisionPro.PMAlign;
using Cognex.VisionPro;
using Cognex.VisionPro.ImageProcessing;
#endif


namespace JzOCR.OPSpace
{
    public class OCRCollectionClass
    {
        const string OCRCollectionPath = @"D:\JETEAZY\OCR";
        string DATACNNSTRING = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + OCRCollectionPath + @"\DB\DATA.mdb;Jet OLEDB:Database Password=12892414;";
        protected OleDbConnection DATACONNECTION;
        protected OleDbCommand DATACOMMAND;
        protected OleDbCommandBuilder DATACMDBUILDER;
        protected OleDbDataAdapter DATAADAPTER;

        protected DataSet DATASET;

        DataTable myDataTable;
     public   List<OCRClass> myDataList = new List<OCRClass>();

        int Index = 0;

        OCRClass DataNull = new OCRClass();
        public OCRClass DataLast
        {
            get
            {
                return myDataList[myDataList.Count - 1];
            }
        }
        public void GotoIndex(int index)
        {
            Index = index;
        }
        public int FindIndex(int no)
        {
            int ret = -1;
            int i = 0;

            while (i < myDataList.Count)
            {
                if (myDataList[i].No == no)
                {
                    ret = i;
                    break;
                }
                i++;
            }

            return ret;
        }
        public OCRClass DataNow
        {
            get
            {
                if (Index == -1)
                    return DataNull;
                else
                {
                    return myDataList[Index];
                }
            }
        }
        public OCRCollectionClass()
        {

        }
        public void Initial()
        {
            string SQLCMD = "";

            DATASET = new DataSet();

            DATACONNECTION = new OleDbConnection(DATACNNSTRING);
            DATACONNECTION.Open();

            SQLCMD = "select * from OCRDB order by OCRDB.no";
            DATAADAPTER = new OleDbDataAdapter();
            DATACMDBUILDER = new OleDbCommandBuilder(DATAADAPTER);
            DATACOMMAND = new OleDbCommand();
            DATACOMMAND.Connection = DATACONNECTION;

            DATACMDBUILDER.QuotePrefix = "[";
            DATACMDBUILDER.QuoteSuffix = "]";

            DATAADAPTER.SelectCommand = new OleDbCommand(SQLCMD, DATACONNECTION);
            DATAADAPTER.Fill(DATASET, "OCRDB");

            myDataTable = DATASET.Tables["OCRDB"];

            Load();
        }
        public void Load()
        {
            myDataList.Clear();

            foreach (DataRow datarow in myDataTable.Rows)
            {
                OCRClass data = new OCRClass();

                data.SetOCRPath(OCRCollectionPath);

                Mapping(datarow, data);

                data.Load(OCRCollectionPath);
                data.TrainPar();
                myDataList.Add(data);
            }

            Index = 0;
        }
        public void Load(int no)
        {
            foreach (DataRow datarow in myDataTable.Rows)
            {
                if ((int)datarow["No"] == no)
                {
                    OCRClass data = new OCRClass();
                    Mapping(datarow, data);
                    data.Load(OCRCollectionPath);
                    data.TrainPar();
                    int i = 0;

                    foreach (OCRClass ocr in myDataList)
                    {
                        if (ocr.No == no)
                            break;

                        i++;
                    }

                    myDataList.RemoveAt(i);
                    myDataList.Insert(i, data);

                    break;
                }
            }

        }
        public void Save()
        {
            int i = 0;

            foreach (OCRClass data in myDataList)
            {
                DataRow datarow = myDataTable.Rows[i];
                Mapping(data, datarow, -1);

                data.Save();
                data.TrainPar();

                i++;
            }

            DATAADAPTER.Update(DATASET, "OCRDB");

        }
        public void Save(int no)
        {
            int i = 0;

            foreach (OCRClass data in myDataList)
            {
                DataRow datarow = myDataTable.Rows[i];

                if ((int)datarow["No"] == no)
                {
                    Mapping(data, datarow, -1);
                    data.Save();
                    data.TrainPar();
                    break;
                }
                i++;
            }
            DATAADAPTER.Update(DATASET, "OCRDB");

        }
        public string[] ToOCRComboItem()
        {
            int i = 0;

            string[] strs = new string[myDataList.Count];

            foreach (OCRClass ocr in myDataList)
            {
                strs[i] = "[" + ocr.No.ToString("000") + "] " + ocr.Name;
                i++;
            }
            return strs;
        }
        public void Mapping(OCRClass fromadata, DataRow todatarow, int assignno)
        {
            if (assignno != -1)
                todatarow["No"] = assignno;
            else
                todatarow["No"] = fromadata.No;

            todatarow["Name"] = fromadata.Name;

            todatarow["OCRMethod"] = fromadata.FromOCRMethodList();
            todatarow["OCRItem"] = fromadata.FromOCRItemList();

            todatarow["Shape"] = fromadata.RectEAG.ToString();

            todatarow["Remark"] = fromadata.Remark;
            todatarow["StartDatetime"] = fromadata.StartDatetime;
            todatarow["ModifyDatetime"] = fromadata.ModifyDatetime;

        }
        public void Mapping(DataRow fromdatarow, OCRClass todata)
        {
            todata.No = (int)fromdatarow["No"];

            string OCRMethodString = fromdatarow["OCRMethod"].ToString();
            string OCRItemString = fromdatarow["OCRItem"].ToString();

            todata.Name = (string)fromdatarow["Name"];
            todata.Remark = (string)fromdatarow["Remark"];

            string RectEAGString = fromdatarow["Shape"].ToString();

            if (RectEAGString != "")
            {
                todata.RectEAG.FromString(RectEAGString);

            }
            todata.StartDatetime = (string)fromdatarow["StartDatetime"];
            todata.ModifyDatetime = (string)fromdatarow["ModifyDatetime"];

            todata.ToOCRMethodList(OCRMethodString);
            todata.ToOCRItemList(OCRItemString);
        }
        public void Add(string ocrnostring, bool iscopy)
        {
            Add(FindIndex(DataNow.No), ocrnostring, iscopy);
        }
        public void Add(int index, string ocrnostring, bool iscopy)
        {
            int LastNo = DataLast.No + 1;
            DataRow newdatarow = myDataTable.NewRow();

            Mapping(myDataList[index], newdatarow, LastNo);

            newdatarow["Name"] = ((string)newdatarow["Name"])[0] + JzTimes.TimeSerialString;

            if (!iscopy)
            {
                newdatarow["OCRMethod"] = "";
                newdatarow["OCRItem"] = "";
            }

            newdatarow["StartDatetime"] = JzTimes.DateTimeString;
            newdatarow["ModifyDatetime"] = JzTimes.DateTimeString;

            myDataTable.Rows.Add(newdatarow);

            OCRClass newocr = new OCRClass();
            Mapping(newdatarow, newocr);

            newocr.SetOCRPath(OCRCollectionPath);

            //newocr.bmpLast.Dispose();
            //newocr.bmpLast = new Bitmap(myDataList[index].bmpLast);

            myDataList.Add(newocr);

            Index = myDataList.Count - 1;

            Copy(OCRCollectionPath + "\\PIC\\" + ocrnostring, OCRCollectionPath + "\\PIC\\" + newocr.OCRNoString);

            newocr.Load(OCRCollectionPath);
        }
        public void Delete(int index)
        {
            string strFile = OCRCollectionPath + "\\PIC\\" + myDataList[index].OCRNoString;
            if (Directory.Exists(strFile))
                Directory.Delete(strFile, true);

            string deletestring = "DELETE FROM " + myDataTable.TableName + " WHERE " + myDataTable.TableName + ".No = " + myDataList[index].No;
            myDataList.RemoveAt(index);
            myDataTable.Rows.RemoveAt(index);

            DATACOMMAND.CommandText = deletestring;
            DATACOMMAND.ExecuteNonQuery();
        }
        public void DeleteLast(int originno)
        {
            Delete(myDataList.Count - 1);
            GotoIndex(FindIndex(originno));
        }
        public bool CheckIsDuplicate(string namestr, int checkno)
        {
            bool ret = false;

            foreach (OCRClass data in myDataList)
            {
                if (data.No == checkno)
                    continue;

                if (data.Name.Trim().ToUpper() == namestr.Trim().ToUpper())
                {
                    ret = true;
                    break;
                }
            }
            return ret;
        }

        /// <summary>
        /// Copy Tree 的功能
        /// </summary>
        /// <param name="fromdir"></param>
        /// <param name="todir"></param>
        void Copy(string fromdir, string todir)
        {
            DirectoryInfo difrom = new DirectoryInfo(fromdir);
            DirectoryInfo dito = new DirectoryInfo(todir);

            CopyAll(difrom, dito);
        }
        void CopyAll(DirectoryInfo difrom, DirectoryInfo dito)
        {
            if (Directory.Exists(dito.FullName))
                Directory.Delete(dito.FullName, true);

            Directory.CreateDirectory(dito.FullName);
            System.Threading.Thread.Sleep(200);

            foreach (FileInfo fi in difrom.GetFiles())
            {
                fi.CopyTo(Path.Combine(dito.FullName, fi.Name), true);
            }

            foreach (DirectoryInfo disourcesubdir in difrom.GetDirectories())
            {
                DirectoryInfo nextsubdir = dito.CreateSubdirectory(disourcesubdir.Name);
                CopyAll(disourcesubdir, nextsubdir);
            }
        }

        public string OCR(Bitmap bmpOcr)
        {

            try
            {
                string file = "D:\\Jeteazy\\OCR\\ocr.png"; // ☜ jpg, gif, tif, pdf, etc.
                bmpOcr.Save(file, ImageFormat.Png);
                //com_asprise_ocr_setup(0);
                //IntPtr _handle = com_asprise_ocr_start("eng", "fastest");
                //IntPtr ptr = com_asprise_ocr_recognize(_handle.ToInt64(), strMess, -1, -1, -1, -1, -1, "all", "text", "", "|", "=");
                //string s = Marshal.PtrToStringAnsi(ptr).Replace("\n", "");
                //com_asprise_ocr_stop(_handle.ToInt64());
                //com_asprise_ocr_util_delete(_handle.ToInt64(), true);
                asprise_ocr_api.AspriseOCR.SetUp();
                asprise_ocr_api.AspriseOCR.InputLicense("123456", "123456789123456789123456789");
                asprise_ocr_api.AspriseOCR ocr = new asprise_ocr_api.AspriseOCR();
                ocr.StartEngine("eng", asprise_ocr_api.AspriseOCR.SPEED_FASTEST);

                //IntPtr strmess = NoParOCR. OCR(file, -1);
                //return Marshal.PtrToStringAnsi(strmess);

                string s = ocr.Recognize(file, -1, -1, -1, -1, -1, asprise_ocr_api.AspriseOCR.RECOGNIZE_TYPE_TEXT, asprise_ocr_api.AspriseOCR.OUTPUT_FORMAT_PLAINTEXT);

                return s;
            }
            catch (Exception ex)
            {
                JetEazy.LoggerClass.Instance.WriteException(ex);
                return "";
            }
        }
    }

    public class OCRClass
    {
        public static string OrgOCRNoString = "00000";
        
        /// <summary>
        /// 编号
        /// </summary>
        public int No = -1;
        /// <summary>
        /// 名称
        /// </summary>
        public string Name = "";
        /// <summary>
        /// 备注
        /// </summary>
        public string Remark = "";
        /// <summary>
        /// 建立时间
        /// </summary>
        public string StartDatetime = "";
        /// <summary>
        /// 修改时间
        /// </summary>
        public string ModifyDatetime = "";
        /// <summary>
        /// 当前或OCR参数训练时用的图
        /// </summary>
        public Bitmap bmpLast = new Bitmap(1, 1);
        /// <summary>
        /// 手动取图框框
        /// </summary>
        public JzRectEAG RectEAG = new JzRectEAG(Color.FromArgb(60, Color.Red), new RectangleF(10, 10, 200, 100));
        /// <summary>
        /// 训练参数列表
        /// </summary>
        public List<OCRMethdClass> OCRMethodList = new List<OCRMethdClass>();
        /// <summary>
        /// ocr在线编辑列表（做参数临时用）
        /// </summary>
        public List<OCRItemClass> OCROnlineItemList = new List<OCRItemClass>();
        /// <summary>
        /// OCR参数列表
        /// </summary>
        public List<OCRItemClass> OCRItemList = new List<OCRItemClass>();
        /// <summary>
        /// OCR参数实际跑线列表
        /// </summary>
        public List<OCRTrain> OCRItemRUNList = new List<OCRTrain>();
        /// <summary>
        /// OCR实际参数,修改 点确定或取消时，临时储存
        /// </summary>
        public List<OCRTrain> OCRItemRUNListTemp = new List<OCRTrain>();
        BasicSpace.JzFindObjectClass JzFind = new BasicSpace.JzFindObjectClass();

        /// <summary>
        /// 备份用 OCR参数
        /// </summary>
        OCRClass OCRBackup;
        /// <summary>
        /// ocr参数地址
        /// </summary>
        string OCRPath = "";        //这是克隆或复制或添加输入。
        /// <summary>
        /// ocr参数地址
        /// </summary>
        string myOCRPath
        {
            get
            {
                return OCRPath + "\\PIC\\" + No.ToString(OrgOCRNoString);
            }
        }
        /// <summary>
        /// 参数编号字符化
        /// </summary>
        public string OCRNoString
        {
            get
            {
                return No.ToString(OrgOCRNoString);
            }
        }

        /// <summary>
        /// 把存放的老参数还原回来
        /// </summary>
        public void ToOCRRunPar()
        {
            OCRItemRUNList.Clear();
            foreach(OCRTrain train in OCRItemRUNListTemp)
            {
                OCRTrain myTemp = train.Clone();
                OCRItemRUNList.Add(myTemp);
            }
            OCRItemRUNListTemp.Clear();
        }
        /// <summary>
        /// 把老参数存放起来
        /// </summary>
        public void ToOCRRunParTemp()
        {
            OCRItemRUNListTemp.Clear();
            foreach (OCRTrain train in OCRItemRUNList )
            {
                OCRTrain myTemp = train.Clone();
                OCRItemRUNListTemp.Add(myTemp);
            }
        }
        public OCRClass()
        {


        }
        /// <summary>
        /// 备份
        /// </summary>
        public void Backup()
        {
            if (OCRBackup == null)
                OCRBackup = new OCRClass();

            OCRBackup.Suicide();
            OCRBackup.Clone(this);
        }
        /// <summary>
        /// 备份参数还原
        /// </summary>
        public void Restore()
        {
            this.Clone(OCRBackup);
            OCRBackup.Suicide();
        }
        /// <summary>
        /// 参数赋值
        /// </summary>
        /// <param name="ocr">赋值的源参数</param>
        void Clone(OCRClass ocr)
        {
            No = ocr.No;
            Name = ocr.Name;
            Remark = ocr.Remark;
            StartDatetime = ocr.StartDatetime;
            ModifyDatetime = ocr.ModifyDatetime;

            OCRPath = ocr.OCRPath;

            bmpLast.Dispose();
            bmpLast = new Bitmap(ocr.bmpLast);
            RectEAG.FromString(ocr.RectEAG.ToString());

            foreach (OCRMethdClass ocrmethod in OCRMethodList)
            {
                ocrmethod.Suicide();
            }
            OCRMethodList.Clear();

            foreach (OCRItemClass ocritem in OCRItemList)
            {
                ocritem.Suicide();
            }
            OCRItemList.Clear();

            foreach (OCRMethdClass ocrmethod in ocr.OCRMethodList)
            {
                OCRMethodList.Add(ocrmethod.Clone());
            }
            foreach (OCRItemClass ocritem in ocr.OCRItemList)
            {
                OCRItemList.Add(ocritem.Clone());
            }
        }
        /// <summary>
        /// 参数地址赋值
        /// </summary>
        /// <param name="ocrpath">参数地址</param>
        public void SetOCRPath(string ocrpath)
        {
            OCRPath = ocrpath;
        }
        /// <summary>
        /// 载入参数
        /// </summary>
        /// <param name="ocrcollectionpath"></param>
        public void Load(string ocrcollectionpath)
        {
            //OCRPath = ocrcollectionpath + "\\PIC\\" + No.ToString(OrgOCRNoString);

            GetBMP(myOCRPath + "\\LAST.png", ref bmpLast);

            if (!File.Exists(myOCRPath + "\\Par.txt"))
            {
                FileStream myFs = new FileStream(myOCRPath + "\\Par.txt", FileMode.Create);
                StreamWriter mySw = new StreamWriter(myFs);
                mySw.Write("");
                mySw.Close();
                myFs.Close();
            }

                StreamReader sr = new StreamReader(myOCRPath + "\\Par.txt", System.Text.Encoding.Default);
            string strpar = sr.ReadToEnd();
            sr.Dispose();
            LoadParString(strpar);

            foreach (OCRMethdClass ocrmethod in OCRMethodList)
            {
                ocrmethod.Load(myOCRPath);
            }
            foreach (OCRItemClass ocritem in OCRItemList)
            {
                ocritem.Load(myOCRPath);
            }
        }
        /// <summary>
        /// 保存参数
        /// </summary>
        public void Save()
        {
            SaveBMP(myOCRPath + "\\LAST.png", ref bmpLast);

            string strPar = SaveOCRPar();
            FileStream myStream = new FileStream(myOCRPath+"\\Par.txt", FileMode.Create, FileAccess.Write);
            StreamWriter sWriter = new StreamWriter(myStream);
            sWriter.Write(strPar);
            sWriter.Close();
            myStream.Close();


            SaveOCRMethod();
            SaveOCRItem();
        }
        /// <summary>
        /// 保存训练参数
        /// </summary>
        void SaveOCRMethod()
        {
            foreach (OCRMethdClass ocrmethod in OCRMethodList)
            {
                ocrmethod.Save(myOCRPath);
            }
        }
        /// <summary>
        /// 保存OCR参数
        /// </summary>
        void SaveOCRItem()
        {
            foreach (OCRItemClass ocritem in OCRItemList)
            {
                ocritem.Save(myOCRPath);
            }
        }
        /// <summary>
        /// 字符转训练参数
        /// </summary>
        /// <param name="str"></param>
        public void ToOCRMethodList(string str)
        {
            foreach (OCRMethdClass ocrmethod in OCRMethodList)
            {
                ocrmethod.Suicide();
            }

            OCRMethodList.Clear();

            if (str.Trim() == "")
                return;

            string[] strs = str.Replace(Environment.NewLine, Universal.SeperateCharB.ToString()).Split(Universal.SeperateCharB);

            foreach (string strx in strs)
            {
                OCRMethdClass newocrmethod = new OCRMethdClass(strx);
                //newocrmethod.Load(OCRPath);
                OCRMethodList.Add(newocrmethod);
            }

        }
        /// <summary>
        /// 训练参数字符化
        /// </summary>
        /// <returns></returns>
        public string FromOCRMethodList()
        {
            string str = "";

            foreach (OCRMethdClass ocrmethod in OCRMethodList)
            {
                str += ocrmethod.ToString() + Environment.NewLine;
            }

            if (str.Length > 0)
                str = str.Remove(str.Length - 2, 2);

            return str;
        }
        /// <summary>
        /// 字符转OCR参数
        /// </summary>
        /// <param name="str"></param>
        public void ToOCRItemList(string str)
        {
            foreach (OCRItemClass ocritem in OCRItemList)
            {
                ocritem.Suicide();
            }

            OCRItemList.Clear();

            if (str.Trim() == "")
                return;

            string[] strs = str.Replace(Environment.NewLine, Universal.SeperateCharB.ToString()).Split(Universal.SeperateCharB);

            foreach (string strx in strs)
            {
                OCRItemClass newocritem = new OCRItemClass(strx);
                //newocritem.Load(OCRPath);
                OCRItemList.Add(newocritem);
            }
        }
        /// <summary>
        /// OCR参数字符化
        /// </summary>
        /// <returns></returns>
        public string FromOCRItemList()
        {
            string str = "";

            foreach (OCRItemClass ocritem in OCRItemList)
            {
                str += ocritem.ToString() + Environment.NewLine;
            }

            if (str.Length > 0)
                str = str.Remove(str.Length - 2, 2);

            return str;
        }
        /// <summary>
        /// 资源清理
        /// </summary>
        public void Suicide()
        {
            bmpLast.Dispose();

            foreach (OCRMethdClass ocrmethod in OCRMethodList)
            {
                ocrmethod.Suicide();
            }
            foreach (OCRItemClass ocritem in OCRItemList)
            {
                ocritem.Suicide();
            }

            OCRMethodList.Clear();
            OCRItemList.Clear();
        }
        public string ToModifyString()
        {
            return "Start Time: " + StartDatetime + Environment.NewLine + " Modify Time: " + ModifyDatetime;
        }
        /// <summary>
        /// OCR参数字符化
        /// </summary>
        /// <returns></returns>
        string SaveOCRPar()
        {
            string strPar = "";
            strPar += iBright + ";";
            strPar += iContrast + ";";
            strPar += iColorDifference + ";";
            strPar += fDifference + ";";
            strPar += fExcludeScore + ";";
            strPar += (isJetOCR ? "1" : "0") + ";";
            strPar += (isNoParOCR ? "1" : "0") + ";";
            strPar += (isDefect ? "1" : "0") + ";";
            strPar += iPoint + ";";
            strPar += iArea + ";";
            return strPar;
        }
        /// <summary>
        /// 字符参数化
        /// </summary>
        /// <param name="myPar"></param>
        void LoadParString(string myPar)
        {
            string[] strpar = myPar.Split(';');
            if (strpar.Length > 6)
            {
                iBright = int.Parse(strpar[0]);
                iContrast = int.Parse(strpar[1]);
                iColorDifference = int.Parse(strpar[2]);
                fDifference = float.Parse(strpar[3]);
                fExcludeScore = float.Parse(strpar[4]);

                if (strpar[5] == "1")
                    isJetOCR = true;
                else
                    isJetOCR = false;
                if (strpar[6] == "1")
                    isNoParOCR = true;
                else
                    isNoParOCR = false;

            }
            if(strpar.Length > 9)
            {
                if (strpar[7] == "1")
                    isDefect = true;
                else
                    isDefect = false;

                iPoint = int.Parse(strpar[8]);
                iArea = int.Parse(strpar[9]);
            }
        }
        /// <summary>
        /// 重置
        /// </summary>
        public void Reset()
        {
            RectEAG.IsFirstSelected = false;
            RectEAG.IsSelected = false;

            foreach (OCRMethdClass ocrmethod in OCRMethodList)
            {
                ocrmethod.RectEAG.IsSelected = false;
                ocrmethod.RectEAG.IsFirstSelected = false;
            }
        }

        #region Tools Operation
        /// <summary>
        /// 载入图片
        /// </summary>
        /// <param name="bmpfilestr">图片地址</param>
        /// <param name="bmp">载入的图片</param>
        void GetBMP(string bmpfilestr, ref Bitmap bmp)
        {
            Bitmap bmptmp = new Bitmap(bmpfilestr);

            bmp.Dispose();
            bmp = new Bitmap(bmptmp);

            bmptmp.Dispose();
        }
        /// <summary>
        /// 保存图片
        /// </summary>
        /// <param name="bmpfilestr">保存地址</param>
        /// <param name="bmp">要保存的图片</param>
        void SaveBMP(string bmpfilestr, ref Bitmap bmp)
        {
            Bitmap bmptmp = new Bitmap(bmp);

            bmptmp.Save(bmpfilestr, Universal.GlobalImageFormat);

            bmptmp.Dispose();
        }

        //public static T Clone<T>(T RealObject)
        //{
        //    using (Stream objectStream = new MemoryStream())
        //    {
        //        System.Runtime.Serialization.IFormatter formatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
        //        formatter.Serialize(objectStream, RealObject);
        //        objectStream.Seek(0, SeekOrigin.Begin);
        //        return (T)formatter.Deserialize(objectStream);
        //    }
        //}
        #endregion

        /// <summary>
        /// 训练参数
        /// 加载参数或参数修改后，都需要调用此方法
        /// </summary>
        public void TrainPar()
        {
            bool istrain = true;
            if (OCRItemList.Count > 0)
            {
                OCRItemRUNList.Clear();
                foreach (OCRItemClass ocr in OCRItemList)
                {
                    OCRTrain train = new OCRTrain();
                    train.bmpItem = ocr.bmpItem.Clone(new Rectangle(0,0,ocr.bmpItem.Width,ocr.bmpItem.Height), PixelFormat.Format24bppRgb);
                    train.strValue = ocr.strRelateName;
                    train.strValue2 = ocr.strRelateName2;
                    train.iBackColor = ocr.iBackColor;
                   bool bol=train.Train();
                    if (!bol)
                        istrain = false;
                 //   train.TrainMarch();
                    OCRItemRUNList.Add(train);
                }
            }

          
        }
        /// <summary>
        /// 拆分图片
        /// </summary>
        /// <param name="bmpMethod">要拆分的图</param>
        /// <param name="bmpFind">拆分的地方</param>
        /// <returns></returns>
        public List<OCRItemClass> SplitToOCRTrain(Bitmap bmpMethod, ref Bitmap bmpFind)
        {
            Bitmap bmp = new Bitmap(bmpFind);
            if (iBright != 0 | iContrast != 0)
                myImageProcessor.SetBrightContrastR(bmp, iBright, iContrast);
            int ibackColor=0;
            int iThr = myImageProcessor.Balance(bmp, ref bmp, ref ibackColor, myImageProcessor.EnumThreshold.Intermodes);
         //    bmp.Save(@"D:\\HAHA LV0.BMP", ImageFormat.Bmp);
            JzFind.SetThreshold(bmp, new Rectangle(0, 0, bmp.Width, bmp.Height), 0, 10, 0, true);
            JzFind.Find(bmp, Color.Red);
        //    bmp.Save(@"D:\\HAHA LV1.BMP", ImageFormat.Bmp);
            JzFind.SortByX();

            SolidBrush grayBrush = new SolidBrush(Color.Red);
            SolidBrush grayBrush2 = new SolidBrush(Color.Lime);
            Graphics g = Graphics.FromImage(bmpFind);

            List<OCRItemClass> mylist = new List<OCRItemClass>();
            if (JzFind.Count > 0)
            {
                foreach (FoundClass found in JzFind.FoundList)
                {
                    if (found.Area > 100)
                    {
                        Rectangle rect;//= found.rect;
                        rect = Rectangle.Inflate(found.rect, 3, 4);
                        if (rect.X < 0)
                            rect.X = 0;
                        if (rect.Y < 0)
                            rect.Y = 0;
                        if ((rect.Width + rect.X) > bmp.Width)
                            rect.Width = bmp.Width - rect.X;
                        if ((rect.Height + rect.Y) > bmp.Height)
                            rect.Height = bmp.Height - rect.Y;

                        //      bmptemp.Save(@"D:\TESTTEST\" + found.rect.ToString() + ".bmp", ImageFormat.Bmp);
                       
                        Bitmap bmpT = bmpMethod.Clone(rect, PixelFormat.Format24bppRgb);
                        OCRItemClass ocr = new OCRItemClass();
                        ocr.bmpItem = bmpT;
                        ocr.rect = rect;
                        ocr.iBackColor = ibackColor;
                        ocr.strRelateName = "?";

                        OCRRUNONE(ocr,false);
                        if (ocr.strRelateName == "?")
                        {
                            mylist.Add(ocr);
                            g.DrawRectangle(new Pen(grayBrush, 1), ocr.rect);
                        }
                        else
                        {
                            mylist.Add(ocr);
                            g.DrawRectangle(new Pen(grayBrush2, 1), ocr.rect);
                            g.DrawString(ocr.strRelateName,
                                new Font("隶书", 10),
                                Brushes.Red, 
                                new Point(ocr.rect.X + ocr.rect.Width/2-5, ocr.rect.Y + ocr.rect.Height));
                        }
                    }
                }
            }
            g.Dispose();
            return mylist;

        }
        /// <summary>
        /// 拆分图片并设别
        /// </summary>
        /// <param name="bmpMethod">要拆分的图</param>
        /// <param name="bmpFind">拆分的地方</param>
        /// <returns></returns>
        public List<OCRItemClass> SplitToOCRTrainSet(Bitmap bmpMethod)
        {
            List<OCRItemClass> mylist = SplitImagesFind(bmpMethod);
             
                foreach (OCRItemClass ocr in mylist)
                {
                   

                        //      bmptemp.Save(@"D:\TESTTEST\" + found.rect.ToString() + ".bmp", ImageFormat.Bmp);

                        //Bitmap bmpT = bmpMethod.Clone(rect, PixelFormat.Format24bppRgb);
                        //OCRItemClass ocr = new OCRItemClass();
                        //ocr.bmpItem = bmpT;
                        //ocr.rect = rect;
                        //ocr.iBackColor = ibackColor;
                        //ocr.strRelateName = "?";

                        OCRRUNONE(ocr, false);
                        //if (ocr.strRelateName == "?")
                        //{
                        //    mylist.Add(ocr);
                        //    g.DrawRectangle(new Pen(grayBrush, 1), ocr.rect);
                        //}
                        //else
                        //{
                        //    mylist.Add(ocr);
                        //    g.DrawRectangle(new Pen(grayBrush2, 1), ocr.rect);
                        //    g.DrawString(ocr.strRelateName,
                        //        new Font("隶书", 10),
                        //        Brushes.Red,
                        //        new Point(ocr.rect.X + ocr.rect.Width / 2 - 5, ocr.rect.Y + ocr.rect.Height));
                        //}
                    }
                
            return mylist;

        }
        /// <summary>
        /// 拆分图片
        /// </summary>
        /// <param name="bmpMethod"></param>
        /// <returns></returns>
        public List<OCRItemClass> SplitImagesFind(Bitmap bmpMethod)
        {
            Bitmap bmpabc = bmpMethod.Clone(new Rectangle(0, 0, bmpMethod.Width, bmpMethod.Height), PixelFormat.Format24bppRgb);
            int tempbackcolorTemp = 0;
            myImageProcessor.Balance(bmpabc, ref bmpabc, ref tempbackcolorTemp, myImageProcessor.EnumThreshold.Minimum);
            bmpabc.Dispose();


            int brlightTemp = 0;
            if (tempbackcolorTemp >= 110 && tempbackcolorTemp < 130)
                brlightTemp = 10;
            else if (tempbackcolorTemp >= 100 && tempbackcolorTemp<110)
                brlightTemp = 20;
            else if (tempbackcolorTemp >= 80 && tempbackcolorTemp < 100)
                brlightTemp = 30;
            else if (tempbackcolorTemp >= 60 && tempbackcolorTemp < 80)
                brlightTemp = 50;
            else if (tempbackcolorTemp < 60)
                brlightTemp = 60;

            Bitmap bmpACC = bmpMethod.Clone(new Rectangle(0, 0, bmpMethod.Width, bmpMethod.Height), PixelFormat.Format24bppRgb);


               //   bmpACC.Save("D:\\bmpacc1.png");
            if (brlightTemp != 0)
                myImageProcessor.SetBrightContrastR(bmpACC, brlightTemp, brlightTemp - 10, true);

      //    bmpACC.Save("D:\\bmpacc2.png");
            Bitmap bmp = new Bitmap(bmpACC);
            if (iBright != 0 | iContrast != 0)
                myImageProcessor.SetBrightContrastR(bmp, iBright, iContrast);
            // int iThr = myImageProcessor.Balance(bmp, ref bmp, myImageProcessor.EnumThreshold.Shanbhag);
            int iBackColor = 0;
       //   bmp.Save(@"D:\\Item.BMP", ImageFormat.Bmp);
            JetEazy.BasicSpace.myImageProcessor.Balance(bmp, ref bmp, ref iBackColor, JetEazy.BasicSpace.myImageProcessor.EnumThreshold.Intermodes);

        //   bmp.Save(@"D:\\Item2.BMP", ImageFormat.Bmp);
            JzFind.SetThreshold(bmp, new Rectangle(0, 0, bmp.Width, bmp.Height), 0, 10, 0, true);
            JzFind.Find(bmp, Color.Red);
       //     bmp.Save(@"D:\HAHA LV1.BMP", ImageFormat.Bmp);
            JzFind.SortByX();

            List<OCRItemClass> mylist = new List<OCRItemClass>();
            List<Rectangle> myrectlist = new List<Rectangle>();
            

            int irect = 0;
            int itiem=0;
            int iHeight = 0;
            int iWidthTemp = 0;
            int iwidth_No_W_No_M = 0,itemp_no_w_no_m=0;
            int iwidth_IS_W = 0,itemp_is_W=0;
            int iwidth_IS_M = 0,itemp_is_M=0;
            foreach (OCRTrain train in OCRItemRUNList)
            {

                if(train.strValue.Length==1)
                {
                    if (iWidthTemp < train.bmpItem.Width)
                        iWidthTemp = train.bmpItem.Width;

                    if (train.strValue != "W" && train.strValue != "M")
                    {
                        if (iwidth_No_W_No_M < train.bmpItem.Width)
                            iwidth_No_W_No_M = train.bmpItem.Width;
                        //itemp_no_w_no_m++;
                    }
                    if (train.strValue == "W")
                    {
                     //   if (iwidth_IS_W < train.bmpItem.Width)
                            iwidth_IS_W += train.bmpItem.Width;
                        itemp_is_W++;
                    }
                    if (train.strValue == "M")
                    {
                       // if (iwidth_IS_M < train.bmpItem.Width)
                            iwidth_IS_M += train.bmpItem.Width;
                        itemp_is_M++;
                    }
                }
                iHeight += train.bmpItem.Height;// -14;
                //if (train.strValue != "1")
                //{
                //    if (train.strValue != "W")
                //    {
                //        if (train.strValue.Length == 1)
                //        {
                            itiem++;
                            irect += train.bmpItem.Width;// - 6;
                //        }
                //    }
                //}

                
            }
            if (itiem != 0)
                irect = irect / itiem;
            if (OCRItemRUNList.Count > 0)
                iHeight = iHeight / OCRItemRUNList.Count;
            if (iwidth_IS_M != 0)
                iwidth_IS_M = iwidth_IS_M / itemp_is_M;
            if (iwidth_IS_W != 0)
                iwidth_IS_W = iwidth_IS_W / itemp_is_W;
            //if (iwidth_No_W_No_M != 0)
            //    iwidth_No_W_No_M = iwidth_No_W_No_M / itemp_no_w_no_m;

            if (JzFind.Count > 0)
            {
                foreach (FoundClass found in JzFind.FoundList)
                {
                    if (found.Area > 50)
                    {
                        Rectangle rect= found.rect;
                     if(Math.Abs( iHeight-   rect.Height) < 23)
                        myrectlist.Add(rect);
                  
                    }
                }
            }
            List<Rectangle> myrectTemp = new List<Rectangle>();

           // RectRomber(myrectlist, irect);
            foreach (Rectangle rect in myrectlist)
            {
                //if (rect.Height-iHeight  < -10)
                //    continue;

                //if (Math.Abs(rect.Width - irect * 2) < 6)
                //{
                //    Rectangle rect1 = new Rectangle(rect.X, rect.Y, rect.Width / 2, rect.Height);
                //    Rectangle rect2 = new Rectangle(rect.X+rect.Width/2, rect.Y, rect.Width / 2, rect.Height);
                //    myrectTemp.Add(rect1);
                //    myrectTemp.Add(rect2);
                //}
                //else
                    myrectTemp.Add(rect);
            }
            int iAdd = 0;
            foreach (Rectangle rect in myrectTemp)
            {
                            Rectangle myrect;//= found.rect;
                            myrect = Rectangle.Inflate(rect, 3, 4);
                            if (myrect.X < 0)
                                myrect.X = 0;
                            if (myrect.Y < 0)
                                myrect.Y = 0;
                            if ((myrect.Width + myrect.X) > bmp.Width)
                                myrect.Width = bmp.Width - myrect.X;
                            if ((myrect.Height + myrect.Y) > bmp.Height)
                                myrect.Height = bmp.Height - myrect.Y;

                
                Bitmap bmpT = bmpACC.Clone(myrect, PixelFormat.Format24bppRgb);

                bool isCheckOKFilst = false;
                if (myrectTemp.Count+ iAdd < strBarcode.Length)
                {
                    if (bmpT.Width - iwidth_IS_M > -3 && bmpT.Width - iwidth_IS_W <= -6)
                    {
                        List<Rectangle> myrect2 = new List<Rectangle>();

                        //Bitmap bmpT = bmpMethod.Clone(rect, PixelFormat.Format24bppRgb);
                        OCRItemClass ocr = new OCRItemClass();
                        ocr.bmpItem = bmpT;
                        ocr.rect = rect;
                        ocr.iBackColor = iBackColor;
                        ocr.strRelateName = "?";

                        OCRRUNONE("M", ocr, false);
                        if (ocr.strRelateName != "M")
                        {
                            for (int it = 1; it < 10; it++)
                            {
                                int brlight2 = it * 10 + 20;

                                Bitmap bmpT2 = bmpT.Clone(new Rectangle(0, 0, bmpT.Width, bmpT.Height), PixelFormat.Format24bppRgb);
                                myImageProcessor.SetBrightContrastR(bmpT2, brlight2, brlight2);
                                int iBackColor2 = 0;
                                //bmpT2.Save(@"D:\\Item.BMP", ImageFormat.Bmp);
                                myImageProcessor.Balance(bmpT2, ref bmpT2, ref iBackColor2, myImageProcessor.EnumThreshold.Minimum);
                                //bmpT2.Save(@"D:\\Item2.BMP", ImageFormat.Bmp);
                                JzFind.SetThreshold(bmpT2, new Rectangle(0, 0, bmpT2.Width, bmpT2.Height), 0, 10, 0, true);
                                JzFind.Find(bmpT2, Color.Red);
                                //bmpT2.Save(@"D:\HAHA LV1.BMP", ImageFormat.Bmp);


                                bmpT2.Dispose();
                                JzFind.SortByX();

                                myrect2.Clear();
                                if (JzFind.Count > 0)
                                {
                                    foreach (FoundClass found in JzFind.FoundList)
                                    {
                                        if (found.Area > 40)
                                        {
                                            Rectangle rect2 = found.rect;
                                            if (Math.Abs(iHeight - rect2.Height) < 15)
                                                myrect2.Add(rect2);

                                        }
                                    }
                                }
                                if (myrect2.Count > 1)
                                {
                                    isCheckOKFilst = true;
                                    break;
                                }
                            }

                            if (isCheckOKFilst)
                            {
                                for (int i = 0; i < myrect2.Count; i++)
                                {
                                    for (int j = i + 1; j < myrect2.Count; j++)
                                    {
                                        if (myrect2[i].X > myrect2[j].X)
                                        {
                                            Rectangle temp = myrect2[i];
                                            myrect2[i] = myrect2[j];
                                            myrect2[j] = temp;
                                        }

                                    }
                                }
                                for (int i = 0; i < myrect2.Count; i++)
                                {
                                    Rectangle recttemp2 = new Rectangle(myrect2[i].X - 5, myrect2[i].Y - 5, myrect2[i].Width + 10, myrect2[i].Height + 10);
                                    if (recttemp2.X < 0)
                                        recttemp2.X = 0;
                                    if (recttemp2.Y < 0)
                                        recttemp2.Y = 0;
                                    if (recttemp2.X + recttemp2.Width > bmpT.Width)
                                        recttemp2.Width = bmpT.Width - recttemp2.X;
                                    if (recttemp2.Y + recttemp2.Height > bmpT.Height)
                                        recttemp2.Height = bmpT.Height - recttemp2.Y;

                                    Bitmap bmpTemp = bmpT.Clone(recttemp2, PixelFormat.Format24bppRgb);

                                    //        bmpTemp.Save(@"D:\123.BMP", ImageFormat.Bmp);

                                    OCRItemClass ocr2 = new OCRItemClass();
                                    ocr2.bmpItem = bmpTemp;
                                    ocr2.rect = new Rectangle(myrect.X + recttemp2.X, myrect.Y + recttemp2.Y, recttemp2.Width, recttemp2.Height);
                                    ocr2.strRelateName = "?";
                                    ocr2.iBackColor = iBackColor;
                                    mylist.Add(ocr2);

                                    //bmpTemp.Save("D:\\ddd.bmp");
                                }
                            }
                        }
                        else
                        {
                            isCheckOKFilst = true;
                            mylist.Add(ocr);
                        }
                    }
                    if (Math.Abs(bmpT.Width - iwidth_IS_W) < 6 && !isCheckOKFilst)
                    {
                        List<Rectangle> myrect2 = new List<Rectangle>();

                        //Bitmap bmpT = bmpMethod.Clone(rect, PixelFormat.Format24bppRgb);
                        OCRItemClass ocr = new OCRItemClass();
                        ocr.bmpItem = bmpT;
                        ocr.rect = rect;
                        ocr.iBackColor = iBackColor;
                        ocr.strRelateName = "?";

                        OCRRUNONE("W", ocr, false);
                        if (ocr.strRelateName != "W")
                        {
                            for (int it = 1; it < 10; it++)
                            {
                                int brlight2 = it * 10 + 20;

                                Bitmap bmpT2 = bmpT.Clone(new Rectangle(0, 0, bmpT.Width, bmpT.Height), PixelFormat.Format24bppRgb);
                                myImageProcessor.SetBrightContrastR(bmpT2, brlight2, brlight2);
                                int iBackColor2 = 0;
                                //bmpT2.Save(@"D:\\Item.BMP", ImageFormat.Bmp);
                                myImageProcessor.Balance(bmpT2, ref bmpT2, ref iBackColor2, myImageProcessor.EnumThreshold.Minimum);
                                //bmpT2.Save(@"D:\\Item2.BMP", ImageFormat.Bmp);
                                JzFind.SetThreshold(bmpT2, new Rectangle(0, 0, bmpT2.Width, bmpT2.Height), 0, 10, 0, true);
                                JzFind.Find(bmpT2, Color.Red);
                                //bmpT2.Save(@"D:\HAHA LV1.BMP", ImageFormat.Bmp);


                                bmpT2.Dispose();
                                JzFind.SortByX();

                                myrect2.Clear();
                                if (JzFind.Count > 0)
                                {
                                    foreach (FoundClass found in JzFind.FoundList)
                                    {
                                        if (found.Area > 40)
                                        {
                                            Rectangle rect2 = found.rect;
                                            if (Math.Abs(iHeight - rect2.Height) < 15)
                                                myrect2.Add(rect2);

                                        }
                                    }
                                }
                                if (myrect2.Count > 1)
                                {
                                    isCheckOKFilst = true;
                                    break;
                                }
                            }

                            if (isCheckOKFilst)
                            {
                                for (int i = 0; i < myrect2.Count; i++)
                                {
                                    for (int j = i + 1; j < myrect2.Count; j++)
                                    {
                                        if (myrect2[i].X > myrect2[j].X)
                                        {
                                            Rectangle temp = myrect2[i];
                                            myrect2[i] = myrect2[j];
                                            myrect2[j] = temp;
                                        }

                                    }
                                }
                                for (int i = 0; i < myrect2.Count; i++)
                                {
                                    Rectangle recttemp2 = new Rectangle(myrect2[i].X - 5, myrect2[i].Y - 5, myrect2[i].Width + 10, myrect2[i].Height + 10);
                                    if (recttemp2.X < 0)
                                        recttemp2.X = 0;
                                    if (recttemp2.Y < 0)
                                        recttemp2.Y = 0;
                                    if (recttemp2.X + recttemp2.Width > bmpT.Width)
                                        recttemp2.Width = bmpT.Width - recttemp2.X;
                                    if (recttemp2.Y + recttemp2.Height > bmpT.Height)
                                        recttemp2.Height = bmpT.Height - recttemp2.Y;

                                    Bitmap bmpTemp = bmpT.Clone(recttemp2, PixelFormat.Format24bppRgb);

                                    //        bmpTemp.Save(@"D:\123.BMP", ImageFormat.Bmp);

                                    OCRItemClass ocr2 = new OCRItemClass();
                                    ocr2.bmpItem = bmpTemp;
                                    ocr2.rect = new Rectangle(myrect.X + recttemp2.X, myrect.Y + recttemp2.Y, recttemp2.Width, recttemp2.Height);
                                    ocr2.strRelateName = "?";
                                    ocr2.iBackColor = iBackColor;
                                    mylist.Add(ocr2);

                                    //bmpTemp.Save("D:\\ddd.bmp");
                                }
                            }
                        }
                        else
                        {
                            isCheckOKFilst = true;
                            mylist.Add(ocr);

                        }
                    }

                    if (bmpT.Width > iwidth_No_W_No_M && !isCheckOKFilst)
                    {
                        List<Rectangle> myrect2 = new List<Rectangle>();

                        //     bool isCheckOK = false;
                        for (int it = 1; it < 30; it++)
                        {
                            int brlight2 = it * 3 + 20;

                            Bitmap bmpT2 = bmpT.Clone(new Rectangle(0, 0, bmpT.Width, bmpT.Height), PixelFormat.Format24bppRgb);
                            myImageProcessor.SetBrightContrastR(bmpT2, brlight2, brlight2);
                            int iBackColor2 = 0;
                            //        bmpT2.Save(@"D:\\Item.BMP", ImageFormat.Bmp);
                            myImageProcessor.Balance(bmpT2, ref bmpT2, ref iBackColor2, myImageProcessor.EnumThreshold.Minimum);
                            //         bmpT2.Save(@"D:\\Item2.BMP", ImageFormat.Bmp);
                            //JzFind.SetThreshold(bmpT2, new Rectangle(0, 0, bmpT2.Width, bmpT2.Height), 0, 10, 0, true);
                            //JzFind.Find(bmpT2, Color.Red);

                            JETLIB.JetGrayImg grayimage = new JETLIB.JetGrayImg(bmpT2);
                            JETLIB.JetImgproc.Threshold(grayimage, 10, grayimage);
                            JETLIB.JetBlob jetBlob = new JETLIB.JetBlob();
                            jetBlob.Labeling(grayimage, JETLIB.JConnexity.Connexity8, JETLIB.JBlobLayer.BlackLayer);
                            int icount = jetBlob.BlobCount;

                            List<Rectangle> listrectlist = new List<Rectangle>();
                            for (int i = 0; i < icount; i++)
                            {
                                int iArea = JETLIB.JetBlobFeature.ComputeIntegerFeature(jetBlob, i, JETLIB.JBlobIntFeature.Area);
                                if (iArea > 40)
                                {
                                    //JRotatedRectangleF jetrect = JetBlobFeature.ComputeMinRectangle(jetBlob, i);

                                    int itop = JETLIB.JetBlobFeature.ComputeIntegerFeature(jetBlob, i, JETLIB.JBlobIntFeature.TopMost);
                                    int iLeft = JETLIB.JetBlobFeature.ComputeIntegerFeature(jetBlob, i, JETLIB.JBlobIntFeature.LeftMost);
                                    int iRight = JETLIB.JetBlobFeature.ComputeIntegerFeature(jetBlob, i, JETLIB.JBlobIntFeature.RightMost);
                                    int iBottom = JETLIB.JetBlobFeature.ComputeIntegerFeature(jetBlob, i, JETLIB.JBlobIntFeature.BottomMost);

                                    Rectangle rectTem = new Rectangle(iLeft, itop, iRight - iLeft, iBottom - itop);
                                    listrectlist.Add(rectTem);
                                }
                            }
                            //          bmpT2.Save(@"D:\HAHA LV1.BMP", ImageFormat.Bmp);

                            bmpT2.Dispose();
                            SortByX(listrectlist);

                            myrect2.Clear();
                            if (listrectlist.Count > 0)
                            {
                                bool isResetTest = false;
                                foreach (Rectangle found in listrectlist)
                                {
                                    Rectangle rect2 = found;
                                    if (Math.Abs(iHeight - rect2.Height) < 15)
                                    {
                                        if (rect2.Width < iWidthTemp + 4)
                                            myrect2.Add(rect2);
                                        else
                                            isResetTest = true;
                                    }
                                }
                                if (isResetTest)
                                    continue;
                            }

                            if (myrect2.Count > 1)
                            {
                                isCheckOKFilst = true;
                                iAdd++;
                                break;
                            }
                        }

                        if (isCheckOKFilst)
                        {
                            for (int i = 0; i < myrect2.Count; i++)
                            {
                                for (int j = i + 1; j < myrect2.Count; j++)
                                {
                                    if (myrect2[i].X > myrect2[j].X)
                                    {
                                        Rectangle temp = myrect2[i];
                                        myrect2[i] = myrect2[j];
                                        myrect2[j] = temp;
                                    }

                                }
                            }
                            for (int i = 0; i < myrect2.Count; i++)
                            {
                                Rectangle recttemp2 = new Rectangle(myrect2[i].X - 3, myrect2[i].Y - 5, myrect2[i].Width + 6, myrect2[i].Height + 10);
                                if (recttemp2.X < 0)
                                    recttemp2.X = 0;
                                if (recttemp2.Y < 0)
                                    recttemp2.Y = 0;
                                if (recttemp2.X + recttemp2.Width > bmpT.Width)
                                    recttemp2.Width = bmpT.Width - recttemp2.X;
                                if (recttemp2.Y + recttemp2.Height > bmpT.Height)
                                    recttemp2.Height = bmpT.Height - recttemp2.Y;

                                Bitmap bmpTemp = bmpT.Clone(recttemp2, PixelFormat.Format24bppRgb);

                                //       bmpTemp.Save(@"D:\123.BMP", ImageFormat.Bmp);
                                //bmpT.Save(@"D:\123_2.BMP", ImageFormat.Bmp);

                                OCRItemClass ocr2 = new OCRItemClass();
                                ocr2.bmpItem = bmpTemp;
                                ocr2.rect = new Rectangle(myrect.X + recttemp2.X, myrect.Y + recttemp2.Y, recttemp2.Width, recttemp2.Height);
                                ocr2.strRelateName = "?";
                                ocr2.iBackColor = iBackColor;
                                mylist.Add(ocr2);

                                //bmpTemp.Save("D:\\ddd.bmp");
                            }
                        }
                        else
                        {
                            isCheckOKFilst = true;
                            OCRItemClass ocr = new OCRItemClass();
                            ocr.bmpItem = bmpT;
                            ocr.rect = myrect;
                            ocr.strRelateName = "?";
                            ocr.iBackColor = iBackColor;
                            mylist.Add(ocr);
                        }
                    }
                }

                if( !isCheckOKFilst)
                {
                    OCRItemClass ocr = new OCRItemClass();
                    ocr.bmpItem = bmpT;
                    ocr.rect = myrect;
                    ocr.strRelateName = "?";
                    ocr.iBackColor = iBackColor;
                    mylist.Add(ocr);
                }
            }

            bmp.Dispose();
            return mylist;
        }
        string strBarcode = "123456789ABC";
        void SortByX(List<Rectangle> rectlist)
        {
            for (int i = 0; i < rectlist.Count; i++)
            {
                Rectangle foundi = rectlist[i];
                for (int j = i + 1; j < rectlist.Count; j++)
                {
                    Rectangle foundj = rectlist[j];
                    if (foundi.Location.X > foundj.Location.X)
                    {
                        Rectangle foundtemp = rectlist[j];
                        rectlist[j] = rectlist[i];
                        rectlist[i] = foundtemp;

                    }
                }
            }
        }

        void RectRomber(List<Rectangle> myrect,int width)
        {

            for (int i = 0; i < myrect.Count; i++)
            {
                for (int j = i + 1; j < myrect.Count; j++)
                {
                    if ((width - myrect[j].Width > 10) && (width - myrect[i].Width > 10))
                    {
                        if (myrect[i].IntersectsWith(myrect[j]) || ((myrect[i].X + myrect[i].Width) == myrect[j].X))
                        {
                            myrect[i] = Rectangle.Union(myrect[i], myrect[j]);

                            myrect.RemoveAt(j);
                            RectRomber(myrect,width);
                            return;
                        }
                    }
                }
            }
            
        }
        /// <summary>
        /// OCR跑线
        /// </summary>
        /// <param name="myBmpRun">跑线图</param>
        /// <returns></returns>
        public string OCRRUNLINE(Bitmap myBmpRun)
        {
            Bitmap bmpnell = null;
            bool isResult = false;
            return OCRRUNLINE(myBmpRun, ref bmpnell, ref isResult);
        }

        /// <summary>
        /// OCR跑线
        /// </summary>
        /// <param name="myBmpRun">跑线图</param>
        /// <param name="mybmpErr">跑线过后的错误图</param>
        /// <param name="myDefect">包含的缺失</param>
        /// <returns></returns>
        public string OCRRUNLINE(Bitmap myBmpRun, ref Bitmap mybmpErr, ref bool myDefect)
        {
            //return OCRRun(myBmpRun);

            //System.Diagnostics.Stopwatch timerTO = new System.Diagnostics.Stopwatch();
            //timerTO.Start();
            List<OCRItemClass> myocrlist = new List<OCRItemClass>();
            if (isJetOCR)
            {
                 myocrlist = SplitImagesFind(myBmpRun);

                //Bitmap bmpT = myBmpRun.Clone(new Rectangle(0, 0, myBmpRun.Width, myBmpRun.Height), PixelFormat.Format24bppRgb);
                //OCRItemClass ocr = new OCRItemClass();
                //ocr.bmpItem = bmpT;
                //ocr.rect = new Rectangle(0, 0, myBmpRun.Width, myBmpRun.Height);
                //ocr.strRelateName = "?";
                //myocrlist.Add(ocr);
            }

            else if (!isJetOCR && isNoParOCR)
            {
                string strOcr = aOCR(myBmpRun);
                foreach (OCRTrain ocrt in OCRItemRUNList)
                {
                    if (ocrt.strValue2 != "")
                        strOcr.Replace(ocrt.strValue, ocrt.strValue2);
                }

                myDefect = false;
                return strOcr;
            }

            //timerTO.Stop();
            Graphics gg = null;
            SolidBrush grayBrush = null;
            SolidBrush grayBrush2 = null;
            SolidBrush grayBrush3 = null;
            SolidBrush grayBrush4 = null;
            string strMess = "";
            if (myDefect)
            {
                Bitmap bmpTest=null;
                if (FormSpace.OCRForm. ISDEBUG)
                    bmpTest = new Bitmap(myBmpRun.Width, myBmpRun.Height + 200);
                else
                    bmpTest = new Bitmap(myBmpRun.Width, myBmpRun.Height);

                mybmpErr = new Bitmap(bmpTest);
                bmpTest.Dispose();
                gg = Graphics.FromImage(mybmpErr);
                gg.DrawImage(myBmpRun, new Point(0, 0));
                grayBrush = new SolidBrush(Color.Red);
                grayBrush2 = new SolidBrush(Color.Lime);
                grayBrush3 = new SolidBrush(Color.Yellow);
                grayBrush4 = new SolidBrush(Color.Blue);
            }
            //List<OCRTrain> listTemp = Clone(OCRItemRUNList);
            List<double> dt = new List<double>();
            //double dttimer = 0;
            bool ismyDefect = true;
            foreach (OCRItemClass item in myocrlist)
            {
                //System.Diagnostics.Stopwatch timer = new System.Diagnostics.Stopwatch();
                //timer.Start();
                item.bmpDifference = null;
                OCRRUNONE(item, myDefect);

                if (item.strRelateName == "?")
                {
                    if (isNoParOCR)
                    {
                        string strbar = "";
                        Bitmap bmpocr = new Bitmap(item.bmpItem.Width + 10, item.bmpItem.Height + 10);
                        Graphics g = Graphics.FromImage(bmpocr);
                        g.DrawImage(item.bmpItem, new PointF(5, 5));
                        g.Dispose();
                        strbar = aOCR(bmpocr);

                        if (strbar == null || strbar == "")
                            item.strRelateName = "?";
                        else
                            item.strRelateName = strbar.Replace("\n", "");
                    }

                }
                if (item.strRelateName2 == "")
                    strMess += item.strRelateName;
                else
                    strMess += item.strRelateName2;

                if (myDefect)
                {
                    if (item.bmpDifference != null)
                        gg.DrawImage(item.bmpDifference, item.rect);

                    if (item.strRelateName == "?")
                        gg.DrawRectangle(new Pen(grayBrush, 1), item.rect);
                    else if (isDefect)
                    {
                        if (item.isDefect)
                            gg.DrawRectangle(new Pen(grayBrush2, 1), item.rect);
                        else
                            gg.DrawRectangle(new Pen(grayBrush4, 1), item.rect);
                    }
                    else
                        gg.DrawRectangle(new Pen(grayBrush3, 1), item.rect);


                    Point point=  new Point(item.rect.X + item.rect.Width / 2 - 5, item.rect.Y + item.rect.Height);
                    gg.DrawString(item.strRelateName,
                          new Font("", 10),
                          Brushes.DarkOrange,
                        point);

                    if (FormSpace.OCRForm. ISDEBUG)
                    // if (item.bmpFind != null && item.bmpTrain != null)
                    {
                        gg.DrawImage(item.bmpFind,
                            new Point(item.rect.X, myBmpRun.Height + 2));
                        gg.DrawImage(item.bmpTrain,
                         new Point(item.rect.X, myBmpRun.Height + 60));
                    }
                }

                if (!item.isDefect)
                    ismyDefect = false;

                //    timer.Stop();
                //dt.Add(timer.ElapsedMilliseconds);
                //dttimer += timer.ElapsedMilliseconds;
            }
            if (myDefect)
            {
                gg.Dispose();
                grayBrush.Dispose();
                grayBrush2.Dispose();
                grayBrush3.Dispose();
                grayBrush4.Dispose();
            }
            myDefect = !ismyDefect;
            GC.Collect();
            return strMess;

        }
        /// <summary>
        /// OCR跑线
        /// </summary>
        /// <param name="strdata">源条码</param>
        /// <param name="myBmpRun">跑线图</param>
        /// <param name="mybmpErr">跑线过后的错误图</param>
        /// <returns></returns>
        public string OCRRUNLINE(string strdata, Bitmap myBmpRun, ref Bitmap mybmpErr)
        {
            bool isResult = false;
            return OCRRUNLINE(strdata, myBmpRun, ref mybmpErr, ref isResult);
        }
        /// <summary>
        /// OCR跑线
        /// </summary>
        /// <param name="strdata">源条码</param>
        /// <param name="myBmpRun">跑线图</param>
        /// <param name="mybmpErr">跑线过后的错误图</param>
        /// <param name="myDefect">包含的缺失</param>
        /// <returns></returns>
        public string OCRRUNLINE(string strdata, Bitmap myBmpRun, ref Bitmap mybmpErr, ref bool myDefect)
        {
            bool[] defectlist = new bool[strdata.Length];
            string strResult = OCRRUNLINE(strdata, myBmpRun, ref mybmpErr, ref defectlist);
            myDefect = true;
            foreach (bool isdefect in defectlist)
            {
                if (!isdefect)
                {
                    myDefect = false;
                    break;
                }
            }
            return strResult;
        }
        /// <summary>
        /// OCR跑线
        /// </summary>
        /// <param name="strdata">源条码</param>
        /// <param name="myBmpRun">跑线图</param>
        /// <param name="mybmpErr">跑线过后的错误图</param>
        /// <param name="Defectlist">包含的缺失</param>
        /// <returns></returns>
        public string OCRRUNLINE(string strdata, Bitmap myBmpRun, ref Bitmap mybmpErr, ref bool[] Defectlist)
        {
            Defectlist = new bool[strdata.Length];
            strBarcode = strdata;
            List<OCRItemClass> myocrlist = new List<OCRItemClass>();
            if (isJetOCR)
            {
                myocrlist = SplitImagesFind(myBmpRun);
            }
            else if (!isJetOCR && isNoParOCR)
            {
                string strOcr = aOCR(myBmpRun);
                foreach (OCRTrain ocrt in OCRItemRUNList)
                {
                    if (ocrt.strValue2 != "")
                        strOcr.Replace(ocrt.strValue, ocrt.strValue2);
                }
                // myDefect = false;
                return strOcr;
            }


            Graphics gg = null;
            SolidBrush grayBrush = null;
            SolidBrush grayBrush2 = null;
            SolidBrush grayBrush3 = null;
            SolidBrush grayBrush4 = null;
            string strMess = "";
            //if (myDefect)
            //{
            Bitmap bmpTest = null;
            if (FormSpace.OCRForm.ISDEBUG)
                bmpTest = new Bitmap(myBmpRun.Width, myBmpRun.Height + 200);
            else
                bmpTest = new Bitmap(myBmpRun.Width, myBmpRun.Height);

            mybmpErr = new Bitmap(bmpTest);
            bmpTest.Dispose();
            gg = Graphics.FromImage(mybmpErr);
            gg.DrawImage(myBmpRun, new Point(0, 0));
            grayBrush = new SolidBrush(Color.Red);
            grayBrush2 = new SolidBrush(Color.Lime);
            grayBrush3 = new SolidBrush(Color.Yellow);
            grayBrush4 = new SolidBrush(Color.Blue);
            //}
            bool ismyDefect = true;

            for (int itemp = 0; itemp < myocrlist.Count; itemp++)
            // foreach (OCRItemClass item in myocrlist)
            {
                OCRItemClass item = myocrlist[itemp];
                item.bmpDifference = null;

                if (myocrlist.Count == strdata.Length)
                {
                    string s = strdata.Substring(itemp, 1);
                    OCRRUNONE(s, item, ismyDefect);
                }
                else
                    OCRRUNONE(item, ismyDefect);

                if (item.strRelateName == "?")
                {
                    if (isNoParOCR)
                    {
                        string strbar = "";
                        Bitmap bmpocr = new Bitmap(item.bmpItem.Width + 10, item.bmpItem.Height + 10);
                        Graphics g = Graphics.FromImage(bmpocr);
                        g.DrawImage(item.bmpItem, new PointF(5, 5));
                        g.Dispose();
                        strbar = aOCR(bmpocr);

                        if (strbar == null || strbar == "")
                            item.strRelateName = "?";
                        else
                            item.strRelateName = strbar.Replace("\n", "");
                    }

                }
                if (item.strRelateName2 == "")
                    strMess += item.strRelateName;
                else
                    strMess += item.strRelateName2;

                //if (myDefect)
                //{
                if (item.bmpDifference != null)
                    gg.DrawImage(item.bmpDifference, item.rect);

                if (item.strRelateName == "?")
                    gg.DrawRectangle(new Pen(grayBrush, 1), item.rect);
                else if (isDefect)
                {
                    if (item.isDefect)
                        gg.DrawRectangle(new Pen(grayBrush2, 1), item.rect);
                    else
                        gg.DrawRectangle(new Pen(grayBrush4, 1), item.rect);
                }
                else
                    gg.DrawRectangle(new Pen(grayBrush3, 1), item.rect);


                Point point = new Point(item.rect.X + item.rect.Width / 2 - 5, item.rect.Y + item.rect.Height);
                gg.DrawString(item.strRelateName,
                      new Font("", 10),
                      Brushes.DarkOrange,
                    point);

                if (FormSpace.OCRForm.ISDEBUG)
                // if (item.bmpFind != null && item.bmpTrain != null)
                {
                    gg.DrawImage(item.bmpFind,
                        new Point(item.rect.X, myBmpRun.Height + 2));
                    gg.DrawImage(item.bmpTrain,
                     new Point(item.rect.X, myBmpRun.Height + 60));
                }
                //}
                if (Defectlist.Length == myocrlist.Count)
                    Defectlist[itemp] = item.isDefect;
                //if (!item.isDefect)
                //    ismyDefect = false;
            }

            //if (myDefect)
            //{
            gg.Dispose();
            grayBrush.Dispose();
            grayBrush2.Dispose();
            grayBrush3.Dispose();
            grayBrush4.Dispose();
            //}
            //myDefect = !ismyDefect;
            GC.Collect();
            return strMess;
        }


        /// <summary>
        /// OCR跑线
        /// </summary>
        /// <param name="strdata">源条码</param>
        /// <param name="myBmpRun">跑线图</param>
        /// <param name="mybmpErr">跑线过后的错误图</param>
        /// <param name="ItemList">每一个字符的情况</param>
        /// <returns></returns>
        public string OCRRUNLINE(string strdata, Bitmap myBmpRun, out Bitmap mybmpErr, out OCRItemClass[] ItemList)
        {
            strBarcode = strdata;
            List<OCRItemClass> myocrlist = new List<OCRItemClass>();
            if (isJetOCR)
            {
                myocrlist = SplitImagesFind(myBmpRun);
            }
            else if (!isJetOCR && isNoParOCR)
            {
                string strOcr = aOCR(myBmpRun);
                foreach (OCRTrain ocrt in OCRItemRUNList)
                {
                    if (ocrt.strValue2 != "")
                        strOcr.Replace(ocrt.strValue, ocrt.strValue2);
                }
                // myDefect = false;
                mybmpErr = null;
                ItemList = null;
                return strOcr;
            }


            Graphics gg = null;
            SolidBrush grayBrush = null;
            SolidBrush grayBrush2 = null;
            SolidBrush grayBrush3 = null;
            SolidBrush grayBrush4 = null;
            string strMess = "";
            //if (myDefect)
            //{
            Bitmap bmpTest = null;
            if (FormSpace.OCRForm.ISDEBUG)
                bmpTest = new Bitmap(myBmpRun.Width, myBmpRun.Height + 200);
            else
                bmpTest = new Bitmap(myBmpRun.Width, myBmpRun.Height);

            mybmpErr = new Bitmap(bmpTest);
            bmpTest.Dispose();
            gg = Graphics.FromImage(mybmpErr);
            gg.DrawImage(myBmpRun, new Point(0, 0));
            grayBrush = new SolidBrush(Color.Red);
            grayBrush2 = new SolidBrush(Color.Lime);
            grayBrush3 = new SolidBrush(Color.Yellow);
            grayBrush4 = new SolidBrush(Color.Blue);
            //}
            bool ismyDefect = true;
            ItemList = new OCRItemClass[myocrlist.Count];
            for (int itemp = 0; itemp < myocrlist.Count; itemp++)
            // foreach (OCRItemClass item in myocrlist)
            {
                OCRItemClass item = myocrlist[itemp];
                item.bmpDifference = null;


                if (myocrlist.Count == strdata.Length)
                {
                    string s = strdata.Substring(itemp, 1);

                    //item.bmpItem.Save("D:\\TESTTEST\\" + s + ".png");
                    OCRRUNONE(s, item, ismyDefect);
                }
                else
                    OCRRUNONE(item, ismyDefect);

                if (item.strRelateName == "?")
                {
                    if (isNoParOCR)
                    {
                        string strbar = "";
                        Bitmap bmpocr = new Bitmap(item.bmpItem.Width + 10, item.bmpItem.Height + 10);
                        Graphics g = Graphics.FromImage(bmpocr);
                        g.DrawImage(item.bmpItem, new PointF(5, 5));
                        g.Dispose();
                        strbar = aOCR(bmpocr);

                        if (strbar == null || strbar == "")
                            item.strRelateName = "?";
                        else
                            item.strRelateName = strbar.Replace("\n", "");
                    }

                }
                if (item.strRelateName2 == "")
                    strMess += item.strRelateName;
                else
                    strMess += item.strRelateName2;

                //if (myDefect)
                //{
                if (item.bmpDifference != null)
                    gg.DrawImage(item.bmpDifference, item.rect);

                if (item.strRelateName == "?")
                    gg.DrawRectangle(new Pen(grayBrush, 1), item.rect);
                else if (isDefect)
                {
                    if (item.isDefect)
                        gg.DrawRectangle(new Pen(grayBrush2, 1), item.rect);
                    else
                        gg.DrawRectangle(new Pen(grayBrush4, 1), item.rect);
                }
                else
                    gg.DrawRectangle(new Pen(grayBrush3, 1), item.rect);


                Point point = new Point(item.rect.X + item.rect.Width / 2 - 5, item.rect.Y + item.rect.Height);
                gg.DrawString(item.strRelateName,
                      new Font("", 10),
                      Brushes.DarkOrange,
                    point);

                if (FormSpace.OCRForm.ISDEBUG)
                // if (item.bmpFind != null && item.bmpTrain != null)
                {
                    gg.DrawImage(item.bmpFind,
                        new Point(item.rect.X, myBmpRun.Height + 2));
                    gg.DrawImage(item.bmpTrain,
                     new Point(item.rect.X, myBmpRun.Height + 60));
                }
                //}
                ItemList[itemp] = item.Clone();
                //if (!item.isDefect)
                //    ismyDefect = false;
            }

            //if (myDefect)
            //{
            gg.Dispose();
            grayBrush.Dispose();
            grayBrush2.Dispose();
            grayBrush3.Dispose();
            grayBrush4.Dispose();
            //}
            //myDefect = !ismyDefect;
            GC.Collect();
            return strMess;
        }


        /// <summary>
        /// OCR跑线
        /// </summary>
        /// <param name="myBmpRun">跑线图</param>
        /// <param name="mybmpErr">跑线过后的错误图</param>
        /// <param name="ItemList">每一个字符的情况</param>
        /// <returns></returns>
        public string OCRRUNLINE( Bitmap myBmpRun, out Bitmap mybmpErr, out OCRItemClass[] ItemList)
        {

            List<OCRItemClass> myocrlist = new List<OCRItemClass>();
            if (isJetOCR)
            {
                myocrlist = SplitImagesFind(myBmpRun);
            }
            else if (!isJetOCR && isNoParOCR)
            {
                string strOcr = aOCR(myBmpRun);
                foreach (OCRTrain ocrt in OCRItemRUNList)
                {
                    if (ocrt.strValue2 != "")
                        strOcr.Replace(ocrt.strValue, ocrt.strValue2);
                }
                // myDefect = false;
                mybmpErr = null;
                ItemList = null;
                return strOcr;
            }


            Graphics gg = null;
            SolidBrush grayBrush = null;
            SolidBrush grayBrush2 = null;
            SolidBrush grayBrush3 = null;
            SolidBrush grayBrush4 = null;
            string strMess = "";
            //if (myDefect)
            //{
            Bitmap bmpTest = null;
            if (FormSpace.OCRForm.ISDEBUG)
                bmpTest = new Bitmap(myBmpRun.Width, myBmpRun.Height + 200);
            else
                bmpTest = new Bitmap(myBmpRun.Width, myBmpRun.Height);

            mybmpErr = new Bitmap(bmpTest);
            bmpTest.Dispose();
            gg = Graphics.FromImage(mybmpErr);
            gg.DrawImage(myBmpRun, new Point(0, 0));
            grayBrush = new SolidBrush(Color.Red);
            grayBrush2 = new SolidBrush(Color.Lime);
            grayBrush3 = new SolidBrush(Color.Yellow);
            grayBrush4 = new SolidBrush(Color.Blue);
            //}
            bool ismyDefect = true;
            ItemList = new OCRItemClass[myocrlist.Count];
            for (int itemp = 0; itemp < myocrlist.Count; itemp++)
            // foreach (OCRItemClass item in myocrlist)
            {
                OCRItemClass item = myocrlist[itemp];
                item.bmpDifference = null;

                OCRRUNONE(item, ismyDefect);

                if (item.strRelateName == "?")
                {
                    if (isNoParOCR)
                    {
                        string strbar = "";
                        Bitmap bmpocr = new Bitmap(item.bmpItem.Width + 10, item.bmpItem.Height + 10);
                        Graphics g = Graphics.FromImage(bmpocr);
                        g.DrawImage(item.bmpItem, new PointF(5, 5));
                        g.Dispose();
                        strbar = aOCR(bmpocr);

                        if (strbar == null || strbar == "")
                            item.strRelateName = "?";
                        else
                            item.strRelateName = strbar.Replace("\n", "");
                    }

                }
                if (item.strRelateName2 == "")
                    strMess += item.strRelateName;
                else
                    strMess += item.strRelateName2;

                //if (myDefect)
                //{
                if (item.bmpDifference != null)
                    gg.DrawImage(item.bmpDifference, item.rect);

                if (item.strRelateName == "?")
                    gg.DrawRectangle(new Pen(grayBrush, 1), item.rect);
                else if (isDefect)
                {
                    if (item.isDefect)
                        gg.DrawRectangle(new Pen(grayBrush2, 1), item.rect);
                    else
                        gg.DrawRectangle(new Pen(grayBrush4, 1), item.rect);
                }
                else
                    gg.DrawRectangle(new Pen(grayBrush3, 1), item.rect);


                Point point = new Point(item.rect.X + item.rect.Width / 2 - 5, item.rect.Y + item.rect.Height);
                gg.DrawString(item.strRelateName,
                      new Font("", 10),
                      Brushes.DarkOrange,
                    point);

                if (FormSpace.OCRForm.ISDEBUG)
                // if (item.bmpFind != null && item.bmpTrain != null)
                {
                    gg.DrawImage(item.bmpFind,
                        new Point(item.rect.X, myBmpRun.Height + 2));
                    gg.DrawImage(item.bmpTrain,
                     new Point(item.rect.X, myBmpRun.Height + 60));
                }
                //}
                ItemList[itemp] = item.Clone();
                //if (!item.isDefect)
                //    ismyDefect = false;
            }

            //if (myDefect)
            //{
            gg.Dispose();
            grayBrush.Dispose();
            grayBrush2.Dispose();
            grayBrush3.Dispose();
            grayBrush4.Dispose();
            //}
            //myDefect = !ismyDefect;
            GC.Collect();
            return strMess;
        }

        /// <summary>
        /// OCR跑线（识别1个字符）
        /// </summary>
        /// <param name="myocr">跑线的参数</param>
        /// <param name="xDefect">是否检测缺失</param>
        void OCRRUNONE(string s, OCRItemClass myocr, bool xDefect)
        {
            List<OCRTrain> listTempTrain = new List<OCRTrain>();
            int iBackColor = 0;

            //Bitmap bmpT = (Bitmap)myocr.bmpItem.Clone(new Rectangle(0, 0, myocr.bmpItem.Width, myocr.bmpItem.Height), PixelFormat.Format24bppRgb);
            //Bitmap bmpBlance = new Bitmap(1, 1);
            //JetEazy.BasicSpace.myImageProcessor.Balance(bmpT, ref bmpBlance, ref iBackColor, JetEazy.BasicSpace.myImageProcessor.EnumThreshold.Minimum);
            //JetEazy.BasicSpace.myImageProcessor.SetBimap8To24(bmpT, bmpBlance, ref myocr.iFousColor);
            //bmpBlance.Dispose();

            //   bmpT.Save("D://Find.bmp", ImageFormat.Bmp);
            List<OCRTrain> myListTempRun = new List<OCRTrain>();

            List<string> listTempADD = new List<string>();
            if (s == "8")
                listTempADD.Add("B");
            if (s == "B")
                listTempADD.Add("8");
            if (s == "Z")
                listTempADD.Add("2");
            if (s == "2")
                listTempADD.Add("Z");
            if (s == "5")
                listTempADD.Add("S");
            if (s == "S")
                listTempADD.Add("5");
            if (s == "C")
                listTempADD.Add("G");
            if (s == "G")
                listTempADD.Add("C");
            if (s == "Q")
                listTempADD.Add("0");
            if (s == "0")
                listTempADD.Add("Q");
            if (s == "Q")
                listTempADD.Add("C");
            if (s == "C")
                listTempADD.Add("Q");
            if (s == "D")
                listTempADD.Add("0");
            if (s == "0")
                listTempADD.Add("D");

            //if (s == "P")
            //    iBackColor = iBackColor + 0;

            foreach (OCRTrain train in OCRItemRUNList)
            {
                if (train.strValue == s)
                {
                    //myocr.bmpItem.Save("D:\\testtest\\bmpItem.png");
                    //train.bmpItemTo.Save("D:\\testtest\\bmpItemTo.png");

                    Bitmap bmpTempFind = new Bitmap(myocr.bmpItem);
                    AUGrayImg8 imginputA = new AUGrayImg8();
                    AUUtility.DrawBitmapToAUGrayImg8(bmpTempFind, ref imginputA);
                    AUGrayImg8 imginputB = new AUGrayImg8();
                    AUUtility.DrawBitmapToAUGrayImg8(train.bmpItemTo, ref imginputB);

                    //bmpTempFind.Save("D:\\testtest\\bmpfing1.png");


                    //imginputB.Save("D:\\testtest\\imginputB.png", eImageFormat.eImageFormat_PNG);
                    //imginputA.Save("D:\\testtest\\imginputA.png", eImageFormat.eImageFormat_PNG);

                    AUGrayImg8 imgout = new AUGrayImg8();
                    AUUtility.DrawBitmapToAUGrayImg8(bmpTempFind, ref imgout);
                    AUImage.IntensityTransfer(imginputA, imginputB, imgout);
                    AUUtility.DrawAUGrayImg8ToBitmap(imgout, ref bmpTempFind);

                    //imgout.Save("D:\\testtest\\imgout.png", eImageFormat.eImageFormat_PNG);
                    //bmpTempFind.Save("D:\\testtest\\bmpfing2.png");

                     Bitmap bmpT = (Bitmap)bmpTempFind.Clone(new Rectangle(0, 0, myocr.bmpItem.Width, myocr.bmpItem.Height), PixelFormat.Format24bppRgb);
                    Bitmap bmpBlance = new Bitmap(1, 1);
                    iBackColor = 255;
                   myImageProcessor.Balance(bmpT, ref bmpBlance, ref iBackColor, myImageProcessor.myOCRThreshold);
                    myImageProcessor.SetBimap8To24(bmpT, bmpBlance, ref myocr.iFousColor);
                    bmpBlance.Dispose();
                    bmpBlance.Dispose();
                    imginputA.Dispose();
                    imginputB.Dispose();
                    imgout.Dispose();
                    train.iBackColor = iBackColor;

                    //bmpT.Save("D:\\testtest\\bmpT.png");

                    if (!train.isTiemTest)
                        train.bmpFind = bmpT.Clone() as Bitmap;
                    else
                        train.bmpFind = bmpTempFind.Clone() as Bitmap;

#if (COGNEX)
                    train.bmpFixBmpTO = myocr.bmpItem.Clone() as Bitmap;
#endif
                    bmpT.Dispose();
                    bmpTempFind.Dispose();
                    myListTempRun.Add(train);

                }
                foreach (string temp in listTempADD)
                {
                    if (train.strValue == temp)
                    {
                        Bitmap bmpTempFind = new Bitmap(myocr.bmpItem);
                        AUGrayImg8 imginputA = new AUGrayImg8();
                        AUUtility.DrawBitmapToAUGrayImg8(bmpTempFind, ref imginputA);
                        AUGrayImg8 imginputB = new AUGrayImg8();
                        AUUtility.DrawBitmapToAUGrayImg8(train.bmpItemTo, ref imginputB);

                        AUGrayImg8 imgout = new AUGrayImg8();
                        AUUtility.DrawBitmapToAUGrayImg8(bmpTempFind, ref imgout);
                        AUImage.IntensityTransfer(imginputA, imginputB, imgout);
                        AUUtility.DrawAUGrayImg8ToBitmap(imgout, ref bmpTempFind);
                        Bitmap bmpT = (Bitmap)bmpTempFind.Clone(new Rectangle(0, 0, myocr.bmpItem.Width, myocr.bmpItem.Height), PixelFormat.Format24bppRgb);
                       
                        Bitmap bmpBlance = new Bitmap(1, 1);
                        myImageProcessor.Balance(bmpT, ref bmpBlance, ref iBackColor, JetEazy.BasicSpace.myImageProcessor.myOCRThreshold);
                        myImageProcessor.SetBimap8To24(bmpT, bmpBlance, ref myocr.iFousColor);
                        bmpBlance.Dispose();
                        imginputA.Dispose();
                        imginputB.Dispose();
                        imgout.Dispose();

                        train.iBackColor = iBackColor;

                        if (!train.isTiemTest)
                            train.bmpFind = bmpT.Clone() as Bitmap;
                        else
                            train.bmpFind = bmpTempFind.Clone() as Bitmap;

#if (COGNEX)
                    train.bmpFixBmpTO = myocr.bmpItem.Clone() as Bitmap;
#endif
                        bmpT.Dispose();
                        bmpTempFind.Dispose();
                        myListTempRun.Add(train);
                    }
                }
            }
           
            System.Threading.Tasks.Parallel.ForEach<OCRTrain>(myListTempRun, train =>
            {
                train.fScore = 0;
                train.Find();

                if (train.fScore > 0.1)
                {
                    lock (listTempTrain)
                        listTempTrain.Add(train);
                }
            });
            myListTempRun.Clear();

            if (listTempTrain.Count > 1)
            {
                sOCRTemp = s;
                OCRCheckA_B(myocr, listTempTrain, iBackColor);
            }
            else
                OCRCheckA(myocr, listTempTrain, iBackColor);

            if (listTempTrain.Count > 0)
            {
                myocr.fScore = listTempTrain[0].fScore;
                myocr.strRelateName2 = listTempTrain[0].strValue2;
                myocr.strRelateName = listTempTrain[0].strValue;
                //myocr.bmpItemTo =(Bitmap) listTempTrain[0].bmpItemTo.Clone();
               
                if (isDefect && xDefect && listTempTrain[0].bmpResult != null)
                {
                    OCRDefect(listTempTrain[0]);
                    myocr.iPoint = listTempTrain[0].iPoint;
                    myocr.iArea = listTempTrain[0].iArea;
                    myocr.isDefect = listTempTrain[0].isDefect;
                    myocr.bmpDifference = listTempTrain[0].bmpDifference;
                    myocr.bmpTrain = listTempTrain[0].bmpItem.Clone() as Bitmap;
                    myocr.bmpFind = listTempTrain[0].bmpFind.Clone() as Bitmap;
                }
                else
                    myocr.isDefect = true;
            }
            else
            {
                myocr.fScore = 0;
                myocr.strRelateName = "?";
               
            }
        }

        string sOCRTemp = "";
        /// <summary>
        /// OCR跑线（识别1个字符）
        /// </summary>
        /// <param name="myocr">跑线的参数</param>
        /// <param name="xDefect">是否检测缺失</param>
        void OCRRUNONE(OCRItemClass myocr, bool xDefect)
        {
            List<OCRTrain> listTempTrain = new List<OCRTrain>();
            int iBackColor = 0;

      //     myocr.bmpItem.Save("D://Find1.bmp", ImageFormat.Bmp);
            //Bitmap temp1 = new Bitmap(1, 1);
            //myImageProcessor.Balance(myocr.bmpItem, ref temp1, ref iBackColor, JetEazy.BasicSpace.myImageProcessor.EnumThreshold.Minimum);
            //temp1.Dispose();

            //Bitmap bitmapTempTemp = new Bitmap(myocr.bmpItem.Width + 20, myocr.bmpItem.Height + 20);
            //Graphics ggg = Graphics.FromImage(bitmapTempTemp);
            //ggg.FillRectangle(new SolidBrush(Color.FromArgb(iBackColor, iBackColor, iBackColor)), 
            //    new RectangleF(0, 0, bitmapTempTemp.Width, bitmapTempTemp.Height));
            //ggg.DrawImage(myocr.bmpItem, new PointF(10, 10));
            //ggg.Dispose();
            //myocr.bmpItem = bitmapTempTemp;
       //     myocr.bmpItem.Save("D://Find2.bmp", ImageFormat.Bmp);

            List<OCRTrain> myListTempRun = new List<OCRTrain>();
            //AForge.Imaging.ExhaustiveTemplateMatching templateMatching = new AForge.Imaging.ExhaustiveTemplateMatching(0.8f);

            foreach (OCRTrain train in OCRItemRUNList)
            {
                Bitmap bmpTempFind = new Bitmap(myocr.bmpItem);
                AUGrayImg8 imginputA = new AUGrayImg8();
                AUUtility.DrawBitmapToAUGrayImg8(bmpTempFind, ref imginputA);
                AUGrayImg8 imginputB = new AUGrayImg8();
                AUUtility.DrawBitmapToAUGrayImg8(train.bmpItemTo, ref imginputB);

                AUGrayImg8 imgout = new AUGrayImg8();
                AUUtility.DrawBitmapToAUGrayImg8(bmpTempFind, ref imgout);

               
                AUImage.IntensityTransfer(imginputA, imginputB, imgout);
                AUUtility.DrawAUGrayImg8ToBitmap(imgout, ref bmpTempFind);

                //if (train.strValue == "C")
                //{
                //    bmpTempFind.Save("D:\\Find.png");
                //    train.bmpItemTo.Save("D:\\Item.png");
                //}

                Bitmap bmpT = (Bitmap)bmpTempFind.Clone(new Rectangle(0, 0, myocr.bmpItem.Width, myocr.bmpItem.Height), PixelFormat.Format24bppRgb);
                
                Bitmap bmpBlance = new Bitmap(1, 1);
                myImageProcessor.Balance(bmpT, ref bmpBlance, ref iBackColor, JetEazy.BasicSpace.myImageProcessor.myOCRThreshold);

                //if (train.strValue == "C" && iBackColor<70)
                //{
                //    bmpBlance.Save("D:\\FindB.png");
                //}
                myImageProcessor.SetBimap8To24(bmpT, bmpBlance, ref myocr.iFousColor);
                bmpBlance.Dispose();

               


                train.iBackColor = iBackColor;
                //Bitmap bmpFindTemp;
                if (!train.isTiemTest)
                {
                    train.bmpFind = bmpT.Clone() as Bitmap;
                    //bmpFindTemp = new Bitmap(bmpT, train.bmpItem.Size);
                }
                else
                {
                    train.bmpFind = bmpTempFind.Clone() as Bitmap;
                    //bmpFindTemp = new Bitmap(bmpTempFind, train.bmpItem.Size);
                }

               

                bmpTempFind.Dispose();
                //train.bmpDifference.Dispose();
                //train.bmpResult.Dispose();
#if (COGNEX)
                train.bmpFixBmpTO = myocr.bmpItem.Clone() as Bitmap;

#endif
                //bmpFindTemp = bmpFindTemp.Clone(new Rectangle(0, 0, bmpFindTemp.Width, bmpFindTemp.Height), train.bmpItem.PixelFormat);

                ////  float iMax = 0;
                //var compare = templateMatching.ProcessImage(train.bmpItem, bmpFindTemp);
                //bmpFindTemp.Dispose();

                //if (compare.Length > 0)//&& compare[0].Similarity > iMax)
                //{
                //记录下最相似的
                //iMax = compare[0].Similarity;
                //train.fScore = compare[0].Similarity;

                    if (Math.Abs(train.bmpFind.Width - train.bmpItem.Width) < 12)
                        myListTempRun.Add(train);
                    else
                {

                }
                
                
                //}
                bmpT.Dispose();

                #region 单线程做
                //                float myHeght = Math.Abs(train.bmpFind.Height - train.bmpItem.Height) / (float)train.bmpItem.Height;
                //                if (myHeght < 0.5f)
                //                {
                //                    float myWidth = Math.Abs(train.bmpFind.Width - train.bmpItem.Width) / (float)train.bmpItem.Width;
                //                    if (myWidth < 0.5f)
                //                    {
                //                        train.fScore = 0;
                //                        train.Find();
                //                        if (train.fScore > fExcludeScore)
                //                        {
                //#if (AUVISION)
                //                            float iwid = Math.Abs(train.bmpFind.Width / 2 - train.xResult.fCenterX);
                //                            if (iwid < 10f)
                //                            {
                //                                float iwidY = Math.Abs(train.bmpFind.Height / 2 - train.xResult.fCenterY);
                //                                if (iwidY < 10f)
                //                                {
                //                                    lock (listTempTrain)
                //                                        listTempTrain.Add(train);
                //                                }
                //                            }
                //#endif
                //#if (COGNEX)
                //                                                        float iwid = Math.Abs(train.bmpFind.Width / 2 - (float)train.cogResult.GetPose().);
                //                                                        if (iwid < 20f)
                //                                                        {
                //                                                            float iwidY = Math.Abs(train.bmpFind.Height / 2 - (float)train.cogResult.GetPose().TranslationY);
                //                                                            if (iwidY <20f)
                //                                                            {
                //                            lock (listTempTrain)
                //                                listTempTrain.Add(train);
                //                                                            }
                //                                                        }
                //#endif
                //                        }
                //                    }
                //                }
                #endregion
            }
            //if (myListTempRun.Count > 1)
            //{
            //    SortByOCRSource(myListTempRun);
            //}
            //else if (myListTempRun.Count == 1)
            //{
            //}


                //for (int i = myListTempRun.Count - 1; i >= 20;i-- )
                //myListTempRun.RemoveAt(i);
               
            System.Threading.Tasks.Parallel.ForEach<OCRTrain>(myListTempRun, train =>
            {
                //if (Math.Abs((train.bmpFind.Height+train.setSize.Height) - train.bmpItem.Height) / (float)train.bmpItem.Height < 0.3f)
                {
                    //if (Math.Abs((train.bmpFind.Width+train.setSize.Width) - train.bmpItem.Width) / (float)train.bmpItem.Width < 0.3f)
                    {
                        train.fScore = 0;
                        train.Find();

                        if (train.fScore > 0.1)//fExcludeScore)
                        {
#if(AUVISION)
                            //float iwid = Math.Abs(train.bmpFind.Width / 2 - train.xResult.fCenterX);
                            //if (iwid < 10f)
                            //{
                            //    float iwidY = Math.Abs(train.bmpFind.Height / 2 - train.xResult.fCenterY);
                            //    if (iwidY < 10f)
                            //    {
                                    lock (listTempTrain)
                                        listTempTrain.Add(train);
                            //    }
                            //}
#endif
#if(COGNEX)
                            lock (listTempTrain)
                                listTempTrain.Add(train);
#endif

                        }
                    }
                }
            });

            if (listTempTrain.Count == 0)
            {
                myocr.fScore = 0;
                myocr.strRelateName = "?";

                return;
                Bitmap bmpT = (Bitmap)myocr.bmpItem.Clone(new Rectangle(0, 0, myocr.bmpItem.Width, myocr.bmpItem.Height), PixelFormat.Format24bppRgb);
                Bitmap bmpBlance = new Bitmap(1, 1);
                myImageProcessor.Balance(bmpT, ref bmpBlance, ref iBackColor, JetEazy.BasicSpace.myImageProcessor.myOCRThreshold);

                bmpT.Dispose();
                bmpT = new Bitmap(bmpBlance.Width + 80, bmpBlance.Height + 20);

                Graphics g = Graphics.FromImage(bmpT);
                g.FillRectangle(new SolidBrush(Color.White), new RectangleF(0, 0, bmpT.Width, bmpT.Height));
                g.DrawImage(bmpBlance, new PointF(10, 10));
                g.Dispose();

                myocr.strRelateName = aOCR(bmpT);
                bmpT.Dispose();

                if (myocr.strRelateName == "")
                    myocr.strRelateName = "?";
                return;
            }

            #if(AUVISION)
            else if (listTempTrain.Count > 1)
            {
                ByOCRSource(listTempTrain, 0.4f);
                //SortByOCRSource(listTempTrain);
            }
            //if (listTempTrain.Count > 1)
            //{
            //int iSave = 20;
            //if ((listTempTrain[0].fScore - listTempTrain[1].fScore) > 0.5)
            //    iSave = 0;
            //else
            //    iSave = 1;

            #region 读错
            //E读错
            //string str1 = listTempTrain[0].strValue;
            //if (str1 == "E" || str1 == "F" || str1 == "L")
            //{
            //    if (listTempTrain.Count > 1)
            //    {
            //        string str2 = listTempTrain[1].strValue;
            //        if (str2 == "E" || str2 == "F" || str2 == "L")
            //        {
            //            if (listTempTrain.Count > 2)
            //            {
            //                string str3 = listTempTrain[2].strValue;
            //                if (str3 == "E" || str3 == "F" || str3 == "L")
            //                {
            //                    iSave = 2;
            //                }
            //            }
            //        }
            //    }
            //}

            ////F 读错
            //if (str1 == "T" || str1 == "H")
            //{
            //    if (listTempTrain.Count > 1)
            //    {
            //        string str2 = listTempTrain[1].strValue;
            //        if (str2 == "F" || str2 == "T" || str2 == "H")
            //        {
            //            if (listTempTrain.Count > 2)
            //            {
            //                string str3 = listTempTrain[2].strValue;
            //                if (str3 == "F" || str3 == "T" || str3 == "H")
            //                {
            //                    iSave = 2;
            //                }
            //            }
            //        }
            //    }
            //}
            ////B 读错
            //if (str1 == "F" || str1 == "L")
            //{
            //    if (listTempTrain.Count > 1)
            //    {
            //        string str2 = listTempTrain[1].strValue;
            //        if (str2 == "F" || str2 == "L" || str2 == "B" || str2 == "E")
            //        {
            //            if (listTempTrain.Count > 2)
            //            {
            //                string str3 = listTempTrain[2].strValue;
            //                if (str3 == "B" || str3 == "P" || str3 == "L")
            //                {
            //                    iSave = 2;
            //                }
            //            }
            //        }
            //    }
            //}

            ////R读错
            //if (str1 == "F" || str1 == "R" || str1 == "P" || str1 == "T" || str1 == "B")
            //{
            //    if (listTempTrain.Count > 1)
            //    {
            //        string str2 = listTempTrain[1].strValue;
            //        if (str2 == "F" || str2 == "R" || str2 == "P" || str2 == "T" || str2 == "B")
            //        {
            //            if (listTempTrain.Count > 2)
            //            {
            //                string str3 = listTempTrain[2].strValue;
            //                if (str3 == "F" || str3 == "R" || str3 == "P" || str3 == "T" || str3 == "B")
            //                {
            //                    iSave = 2;
            //                }
            //            }
            //        }
            //    }
            //}
            #endregion

            //for (int i = listTempTrain.Count - 1; i > iSave; i--)
            //    listTempTrain.RemoveAt(i);

            //}
#endif


            if (listTempTrain.Count > 1)
                OCRCheckA_B(myocr, listTempTrain, iBackColor);
            else
                OCRCheckA(myocr, listTempTrain, iBackColor);

            if (listTempTrain.Count > 0)
            {
                myocr.fScore = listTempTrain[0].fScore;
                myocr.strRelateName2 = listTempTrain[0].strValue2;
                myocr.strRelateName = listTempTrain[0].strValue;

                if (isDefect && xDefect && listTempTrain[0].bmpResult!=null)
                {
                    OCRDefect(listTempTrain[0]);
                    myocr.isDefect = listTempTrain[0].isDefect;
                    myocr.iPoint = listTempTrain[0].iPoint;
                    myocr.iArea = listTempTrain[0].iArea;
                    myocr.bmpDifference = listTempTrain[0].bmpDifference;
                    myocr.bmpTrain = listTempTrain[0].bmpItem.Clone() as Bitmap;
                    myocr.bmpFind = listTempTrain[0].bmpFind.Clone() as Bitmap;
                }
                else
                    myocr.isDefect = true;
            }
            else
            {
                myocr.fScore = 0;
                myocr.strRelateName = "?";
                return;
                Bitmap bmpT = (Bitmap)myocr.bmpItem.Clone(new Rectangle(0, 0, myocr.bmpItem.Width, myocr.bmpItem.Height), PixelFormat.Format24bppRgb);

                Bitmap bmpBlance = new Bitmap(1, 1);
                myImageProcessor.Balance(bmpT, ref bmpBlance, ref iBackColor, JetEazy.BasicSpace.myImageProcessor.myOCRThreshold);

                myocr.strRelateName = aOCR(bmpBlance);

                if(myocr.strRelateName == "")
                    myocr.strRelateName = "?";

            }
        }
        /// <summary>
        /// 选择最相近的一个图
        /// </summary>
        /// <param name="bmpTemp">源图</param>
        /// <param name="listResult">优选参数</param>
        void OCRCheckA_B(OCRItemClass myocr, List<OCRTrain> listResult, int iBackColor)
        {
#if(AUVISION)
            AUColorImg24 imginput24 = new AUColorImg24(myocr.bmpItem.Width, myocr.bmpItem.Height);
            AUUtility.DrawBitmapToAUColorImg24(myocr.bmpItem, ref imginput24);
#endif

            for (int i = 0; i < listResult.Count; i++)
            {
                OCRTrain foundi = listResult[i];
                for (int j = i + 1; j < listResult.Count;)
                {
                    OCRTrain foundj = listResult[j];
                    int ibmpA = 0, ibmpB = 0;
#if (AUVISION)
                    bool isave = false;
                    if (foundi.strValue == "XY" && foundj.strValue == "X")
                        isave = false;
                    if (foundj.strValue == "XY" && foundi.strValue == "X")
                        isave = false;

                    Bitmap bmpfind = new Bitmap(foundi.bmpItem.Width, foundi.bmpItem.Height, PixelFormat.Format24bppRgb);
                    Bitmap bmpfindB = new Bitmap(foundj.bmpItem.Width, foundj.bmpItem.Height, PixelFormat.Format24bppRgb);

                    AUColorImg24 imgoutput24 = new AUColorImg24(foundi.bmpItem.Width, foundi.bmpItem.Height);
                    imgoutput24.SetImage(foundi.iBackColor);
                    ScaleRotate(foundi.xResult, imginput24, ref imgoutput24);
                    AUUtility.DrawAUColorImg24ToBitmap(imgoutput24, ref bmpfind);
                    imgoutput24.Dispose();

                    AUColorImg24 imgoutput24B = new AUColorImg24(foundj.bmpItem.Width, foundj.bmpItem.Height);
                    imgoutput24B.SetImage(foundj.iBackColor);
                    ScaleRotate(foundj.xResult, imginput24, ref imgoutput24B);
                    AUUtility.DrawAUColorImg24ToBitmap(imgoutput24B, ref bmpfindB);
                    imgoutput24B.Dispose();


                    Bitmap myBmpItemA = foundi.bmpFind.Clone() as Bitmap;
                    Bitmap bmpfindA = new Bitmap(myBmpItemA.Width, myBmpItemA.Height, PixelFormat.Format24bppRgb);
                    AUColorImg24 imginput24A = new AUColorImg24(myBmpItemA.Width, myBmpItemA.Height);
                    AUUtility.DrawBitmapToAUColorImg24(myBmpItemA, ref imginput24A);

                    AUColorImg24 imgoutput24A = new AUColorImg24(foundi.bmpItem.Width, foundi.bmpItem.Height);
                    imgoutput24A.SetImage(foundi.iBackColor);
                    ScaleRotate(foundi.xResult, imginput24A, ref imgoutput24A);
                    AUUtility.DrawAUColorImg24ToBitmap(imgoutput24A, ref bmpfindA);
                    imgoutput24A.Dispose();
                    imginput24A.Dispose();

                    int ibacktemp = 0;
                    myImageProcessor.Balance(bmpfindA, ref bmpfindA, ref ibacktemp, myImageProcessor.myOCRThreshold);



                    Bitmap myBmpItem_B = foundj.bmpFind.Clone() as Bitmap;
                    Bitmap bmpfind_B = new Bitmap(myBmpItem_B.Width, myBmpItem_B.Height, PixelFormat.Format24bppRgb);
                    AUColorImg24 imginput24_B = new AUColorImg24(myBmpItem_B.Width, myBmpItem_B.Height);
                    AUUtility.DrawBitmapToAUColorImg24(myBmpItem_B, ref imginput24_B);

                    AUColorImg24 imgoutput24_B = new AUColorImg24(foundj.bmpItem.Width, foundj.bmpItem.Height);
                    imgoutput24_B.SetImage(foundj.iBackColor);
                    ScaleRotate(foundj.xResult, imginput24_B, ref imgoutput24_B);
                    AUUtility.DrawAUColorImg24ToBitmap(imgoutput24_B, ref bmpfind_B);
                    imgoutput24_B.Dispose();
                    imginput24_B.Dispose();

                    int ibacktemp2 = 0;
                    myImageProcessor.Balance(bmpfind_B, ref bmpfind_B, ref ibacktemp2, myImageProcessor.myOCRThreshold);


#endif
#if (COGNEX)
                    Bitmap bmpfind = foundi.bmpFix.Clone() as Bitmap;
                    //Bitmap bmpfind = new Bitmap(foundi.bmpFix, foundi.bmpItemTo.Width, foundi.bmpItemTo.Height);
                    //bmpfind = bmpfind.Clone(new Rectangle(0, 0, bmpfind.Width, bmpfind.Height), PixelFormat.Format24bppRgb);

                    Bitmap bmpfindItem = foundi.bmpFixItem.Clone() as Bitmap;
                    //Bitmap bmpfindItem = new Bitmap(foundi.bmpFixItem, foundi.bmpItemTo.Width, foundi.bmpItemTo.Height);
                    //bmpfindItem = bmpfindItem.Clone(new Rectangle(0, 0, bmpfindItem.Width, bmpfindItem.Height), PixelFormat.Format24bppRgb);

                    Bitmap bmpfindB = foundj.bmpFix.Clone() as Bitmap;
                    //Bitmap bmpfindB = new Bitmap(foundj.bmpFix, foundj.bmpItemTo.Width, foundj.bmpItemTo.Height);
                    //bmpfindB = bmpfindB.Clone(new Rectangle(0, 0, bmpfindB.Width, bmpfindB.Height), PixelFormat.Format24bppRgb);

                    Bitmap bmpfindBItem = foundj.bmpFixItem.Clone() as Bitmap;
                    //Bitmap bmpfindBItem = new Bitmap(foundj.bmpFixItem, foundj.bmpItemTo.Width, foundj.bmpItemTo.Height);
                    //bmpfindBItem = bmpfindBItem.Clone(new Rectangle(0, 0, bmpfindBItem.Width, bmpfindBItem.Height), PixelFormat.Format24bppRgb);
#endif
                    if (isave)
                    {
                        //foundi.bmpItemTo.Save("D:\\TESTTEST\\A1Find.png");

                        string strPath = "D:\\TESTTEST\\" + foundi.strValue + "\\";
                        if (false == System.IO.Directory.Exists(strPath))
                            System.IO.Directory.CreateDirectory(strPath);
                        bmpfind.Save(strPath + DateTime.Now.ToString("yyyyMMddHHmmss") + ".png");
                        myBmpItemA.Save(strPath + DateTime.Now.ToString("yyyyMMddHHmmss") + "2.png");
                        foundi.bmpItem.Save("D:\\TESTTEST\\" + foundi.strValue + "Train.png");



                        //bmpfindA.Save("D:\\TESTTEST\\A2Item.png");

                        //foundj.bmpItemTo.Save("D:\\TESTTEST\\B1Find.png");
                        strPath = "D:\\TESTTEST\\" + foundj.strValue + "\\";
                        if (false == System.IO.Directory.Exists(strPath))
                            System.IO.Directory.CreateDirectory(strPath);
                        bmpfindB.Save(strPath + DateTime.Now.ToString("yyyyMMddHHmmss") + ".png");
                        myBmpItem_B.Save(strPath + DateTime.Now.ToString("yyyyMMddHHmmss") + "2.png");

                        foundj.bmpItem.Save("D:\\TESTTEST\\" + foundj.strValue + "Train.png");
                        //bmpfind_B.Save("D:\\TESTTEST\\B2Item.png");
                    }
                    myBmpItemA.Dispose();
                    myBmpItem_B.Dispose();

                    AUGrayImg8 imginputA = new AUGrayImg8();
                    AUUtility.DrawBitmapToAUGrayImg8(bmpfind, ref imginputA);
                    AUGrayImg8 imginputB = new AUGrayImg8();
                    AUUtility.DrawBitmapToAUGrayImg8(foundi.bmpItemTo, ref imginputB);

                    AUGrayImg8 imgout = new AUGrayImg8();
                    AUUtility.DrawBitmapToAUGrayImg8(bmpfind, ref imgout);
                    AUImage.IntensityTransfer(imginputA, imginputB, imgout);
                    AUUtility.DrawAUGrayImg8ToBitmap(imgout, ref bmpfind);

                    imginputA = new AUGrayImg8();
                    AUUtility.DrawBitmapToAUGrayImg8(bmpfindB, ref imginputA);
                    imginputB = new AUGrayImg8();
                    AUUtility.DrawBitmapToAUGrayImg8(foundj.bmpItemTo, ref imginputB);
                    imgout = new AUGrayImg8();
                    AUUtility.DrawBitmapToAUGrayImg8(bmpfindB, ref imgout);

                    AUImage.IntensityTransfer(imginputA, imginputB, imgout);
                    AUUtility.DrawAUGrayImg8ToBitmap(imgout, ref bmpfindB);

                    int iFobacklA = 0, iFobacklB = 0;

#if (COGNEX)
                    myImageProcessor.SetBimap_A_B(bmpfindItem, foundi.bmpItemTo,bmpfind,foundi.bmpItem, out foundi.bmpDifference, iColorDifference, ref ibmpA, ref iFobacklA);
                    myImageProcessor.SetBimap_A_B(bmpfindBItem, foundj.bmpItemTo, bmpfindB, foundj.bmpItem, out foundj.bmpDifference, iColorDifference, ref ibmpB, ref iFobacklB);

                    foundi.bmpResult = bmpfindItem;
                    foundj.bmpResult = bmpfindBItem;
#endif
#if (AUVISION)
                    //myImageProcessor.SetBimap_A_B(bmpfind, foundi.bmpItemTo, out foundi.bmpDifference, iColorDifference, ref ibmpA);
                    //myImageProcessor.SetBimap_A_B(bmpfindB, foundj.bmpItemTo, out foundj.bmpDifference, iColorDifference, ref ibmpB);
                    //foundi.bmpResult = foundi.bmpDifference;
                    //foundj.bmpResult = foundj.bmpDifference;
                    int ibackl = 0;// iBackColor - foundi.iBackColorTemp;
                    myImageProcessor.SetBimap_A_B(bmpfind, foundi.bmpItemTo, foundi.bmpItem, bmpfindA, out foundi.bmpDifference, (int)(iColorDifference * 2.55f), ref ibmpA, ref iFobacklA, ibackl);
                    int ibackl2 = 0;// iBackColor - foundj.iBackColorTemp;
                    myImageProcessor.SetBimap_A_B(bmpfindB, foundj.bmpItemTo, foundj.bmpItem, bmpfind_B, out foundj.bmpDifference, (int)(iColorDifference * 2.55f), ref ibmpB, ref iFobacklB, ibackl2);

                    foundi.bmpResult = bmpfind;
                    foundj.bmpResult = bmpfindB;
#endif
                    if (isave)
                    {
                        if (foundi.bmpDifference != null)
                            foundi.bmpDifference.Save("D:\\TESTTEST\\AbmpDifferenceA.bmp");
                        if (foundj.bmpDifference != null)
                            foundj.bmpDifference.Save("D:\\TESTTEST\\BbmpDifferenceB.bmp");
                    }

                    bmpfind.Dispose();
                    bmpfindB.Dispose();
                    //foundi.bmpResult = bmpfindItem;
                    //foundj.bmpResult = bmpfindBItem;

                    if (ibmpA == ibmpB)
                    {
                        if (foundi.fScore > foundj.fScore)
                            ibmpB++;
                        else
                            ibmpA++;
                    }

                    if ((ibmpA < 20 && ibmpB < 20) &&
                       (foundi.strValue.Length > 1 || foundj.strValue.Length > 1) &&
                       ((foundi.strValue.Length > 1 && Math.Abs(foundi.bmpItem.Width - foundi.bmpFind.Width) < 5) ||
                       (foundj.strValue.Length > 1 && Math.Abs(foundj.bmpItem.Width - foundj.bmpFind.Width) < 5)))
                    {

                        if (foundi.strValue.Length > foundj.strValue.Length)
                        {
                            ibmpA = 0;
                            ibmpB++;
                        }
                        else if (foundi.strValue.Length < foundj.strValue.Length)
                        {
                            ibmpA++;
                            ibmpB = 0;
                        }
                    }
                    else if (Math.Abs(ibmpA - ibmpB) < 5)
                    {

                        if (foundi.fScore > foundj.fScore)
                        {
                            ibmpA = 0;
                            ibmpB = 10;
                        }
                        else
                        {
                            ibmpA = 10;
                            ibmpB = 0;
                        }
                    }
                    
                    bool isRemove = false;
                    //  if ((float)ibmpB / (foundj.bmpItem.Width * foundj.bmpItem.Height) > fDifference)
                    if ((float)ibmpB / iFobacklB> fDifference)
                    {
                        isRemove = true;
                        listResult.RemoveAt(j);
                    }
                    // if ((float)ibmpA / (foundi.bmpItem.Width * foundi.bmpItem.Height) > fDifference)
                    if ((float)ibmpA / iFobacklA > fDifference)
                    {
                        isRemove = true;
                        listResult.RemoveAt(i);
                    }
                    if (!isRemove)
                    {
                        if (listResult.Count > 1)
                        {
                            if (ibmpA >= ibmpB)
                                listResult.RemoveAt(i);
                            else
                                listResult.RemoveAt(j);
                        }

                    }
                    if (listResult.Count > 1)
                        OCRCheckA_B(myocr, listResult, iBackColor);
                    else
                        OCRCheckA(myocr, listResult, iBackColor);
                    #if(AUVISION)
                    imginput24.Dispose();
#endif
                    
                    return;

                }
            }
        }
        /// <summary>
        ///  检查源图与OCR训练图的差异
        /// </summary>
        /// <param name="bmpTemp"></param>
        /// <param name="listResult"></param>
        void OCRCheckA(OCRItemClass myocr, List<OCRTrain> listResult, int iBackColor)
        {
            for (int i = 0; i < listResult.Count; i++)
            {
                OCRTrain foundi = listResult[i];

              

                int ibmpA = 0;
#if (AUVISION)
                bool issave = false;
                if (foundi.strValue == "P")
                {
                    issave = false;
                    //foundi.xResult.fCenterX = 15f;
                }

                iBackColor = foundi.iBackColor;
                if (Math.Abs(foundi.xResult.fCenterX - myocr.bmpItem .Width / 2) > 10)
                {
                    iBackColor = 255;
                }

                Bitmap bmpfind = new Bitmap(foundi.bmpItem.Width, foundi.bmpItem.Height, PixelFormat.Format24bppRgb);
                AUColorImg24 imginput24 = new AUColorImg24(myocr.bmpItem.Width, myocr.bmpItem.Height);
                AUUtility.DrawBitmapToAUColorImg24(myocr.bmpItem, ref imginput24);

                AUColorImg24 imgoutput24 = new AUColorImg24(foundi.bmpItem.Width, foundi.bmpItem.Height);
                imgoutput24.SetImage(foundi.iBackColorTemp);
                

                ScaleRotate(foundi.xResult, imginput24, ref imgoutput24);
                AUUtility.DrawAUColorImg24ToBitmap(imgoutput24, ref bmpfind);
                imgoutput24.Dispose();
                imginput24.Dispose();

                Bitmap myBmpItem = foundi.bmpFind.Clone() as Bitmap;
                Bitmap bmpfind2 = new Bitmap(myBmpItem.Width, myBmpItem.Height, PixelFormat.Format24bppRgb);
                AUColorImg24 imginput242 = new AUColorImg24(myBmpItem.Width, myBmpItem.Height);
                AUUtility.DrawBitmapToAUColorImg24(myBmpItem, ref imginput242);

                AUColorImg24 imgoutput242 = new AUColorImg24(foundi.bmpItem.Width, foundi.bmpItem.Height);

               
                imgoutput242.SetImage(iBackColor);
                ScaleRotate(foundi.xResult, imginput242, ref imgoutput242);
                AUUtility.DrawAUColorImg24ToBitmap(imgoutput242, ref bmpfind2);
                imgoutput242.Dispose();
                imginput242.Dispose();
                myImageProcessor.Balance(bmpfind2, ref bmpfind2, ref iBackColor, myImageProcessor.myOCRThreshold);


#endif
#if (COGNEX)
                Bitmap bmpfind = foundi.bmpFix.Clone() as Bitmap;
                //Bitmap bmpfind = new Bitmap(foundi.bmpFix, foundi.bmpItem.Width, foundi.bmpItem.Height);
                //bmpfind = bmpfind.Clone(new Rectangle(0, 0, bmpfind.Width, bmpfind.Height), PixelFormat.Format24bppRgb);

                Bitmap bmpfindItem = foundi.bmpFixItem.Clone() as Bitmap;
                //Bitmap bmpfindItem = new Bitmap(foundi.bmpFixItem, foundi.bmpItemTo.Width, foundi.bmpItemTo.Height);
                //bmpfindItem = bmpfindItem.Clone(new Rectangle(0, 0, bmpfindItem.Width, bmpfindItem.Height), PixelFormat.Format24bppRgb);
#endif
                

                if (issave)
                {
                    foundi.bmpItem.Save("D:\\TESTTEST\\FindA_A.bmp");
                    bmpfind2.Save("D:\\TESTTEST\\FindB_B.bmp");
                    foundi.bmpItemTo.Save("D:\\TESTTEST\\FindA.bmp");
                    bmpfind.Save("D:\\TESTTEST\\FindB.bmp");
                }

                AUGrayImg8 imginputA = new AUGrayImg8();
                AUUtility.DrawBitmapToAUGrayImg8(bmpfind, ref imginputA);
                AUGrayImg8 imginputB = new AUGrayImg8();
                imginputB.SetImage(foundi.iBackColorTemp);
                AUUtility.DrawBitmapToAUGrayImg8(foundi.bmpItemTo, ref imginputB);

                AUGrayImg8 imgout = new AUGrayImg8();
                AUUtility.DrawBitmapToAUGrayImg8(foundi.bmpItemTo, ref imgout);
                AUImage.IntensityTransfer(imginputA, imginputB, imgout);
                AUUtility.DrawAUGrayImg8ToBitmap(imgout, ref bmpfind);

                if (issave)
                {
                    bmpfind.Save("D:\\TESTTEST\\FindFind.bmp");
                }

                int iFobackl = 0;

                //bmpTemp.Save("D:\\bmptemp.png");
#if (AUVISION)
               //    myImageProcessor.SetBimap_A_B(bmpfind, foundi.bmpItem, out foundi.bmpDifference, iColorDifference, ref ibmpA);
                //    bmpfind.Save("D:\\bmpfind_NORUN.bmp");
                int ibackl = 0;// iBackColor - foundi.iBackColorTemp;
                myImageProcessor.SetBimap_A_B(bmpfind, foundi.bmpItemTo,  foundi.bmpItem, bmpfind2, out foundi.bmpDifference, (int)(iColorDifference * 2.55f), ref ibmpA, ref iFobackl, ibackl);


                foundi.bmpResult = bmpfind;// foundi.bmpDifference;  

#endif
#if (COGNEX)

                myImageProcessor.SetBimap_A_B(bmpfindItem, foundi.bmpItemTo, bmpfind, foundi.bmpItem, out foundi.bmpDifference, iColorDifference, ref ibmpA, ref iFobackl);
                foundi.bmpResult = bmpfindItem;
#endif
                if (issave)
                {
                    if (foundi.bmpDifference != null)
                        foundi.bmpDifference.Save("D:\\TESTTEST\\bmpDifference.bmp");

                    bmpfind.Save("D:\\TESTTEST\\bmpfind.bmp");
                    foundi.bmpItemTo.Save("D:\\TESTTEST\\bmpItemTo.bmp");
                    bmpfind2.Save("D:\\TESTTEST\\bmpfind2.bmp");
                    foundi.bmpItem.Save("D:\\TESTTEST\\foundi.bmpItem.bmp");
                    foundi.bmpFind.Save("d:\\TESTTEST\\find.bmp");
                    myocr.bmpItem.Save("D:\\TESTTEST\\bmpTemp.bmp");
                }
                myocr.bmpItemTo = (Bitmap)foundi.bmpDifference.Clone();
                //   if ((float)ibmpA / (foundi.bmpItem.Width * foundi.bmpItem.Height) > fDifference)
                if ( ibmpA>50  || (float)ibmpA / iFobackl > fDifference)
                {
                  
                    listResult.RemoveAt(i);
                  //  OCRCheckA(bmpTemp, listResult, iBackColor);
                    return;
                }
                if (Math.Abs(foundi.xResult.fCenterX - myocr.bmpItem.Width / 2) > 20)
                {
                    listResult.RemoveAt(i);
                    //  OCRCheckA(bmpTemp, listResult, iBackColor);
                    return;
                }

            }
        }

        JzFindObjectClass myDefFind = new JzFindObjectClass();
        /// <summary>
        /// 检测缺失
        /// </summary>
        /// <param name="MyOCR"></param>
        void OCRDefect(OCRTrain MyOCR)
        {
            myDefFind.SetThreshold(MyOCR.bmpResult, 2, 2, 2);
            myDefFind.Find(MyOCR.bmpResult, Color.Red);
            int iMAX = 0;
            foreach (FoundClass found in myDefFind.FoundList)
            {
                if (iMAX < found.Area)
                    iMAX = found.Area;
            }
            MyOCR.iArea = iMAX;
            MyOCR.iPoint = myDefFind.FoundList.Count;

            if (MyOCR.iArea > iArea || MyOCR.iPoint > iPoint)
                MyOCR.isDefect = false;
            else
                MyOCR.isDefect = true;
        }
        /// <summary>
        /// 用分数排序
        /// </summary>
        /// <param name="listResult"></param>
        void SortByOCRSource(List<OCRTrain> listResult)
        {
            for (int i = 0; i < listResult.Count; i++)
            {
                OCRTrain foundi = listResult[i];
                for (int j = i + 1; j < listResult.Count; j++)
                {
                    OCRTrain foundj = listResult[j];
                    if (foundi.fScore < foundj.fScore)
                    {
                        OCRTrain foundtemp = foundj;
                        listResult[j] = foundi;
                        listResult[i] = foundtemp;

                        foundi = listResult[i];
                        foundj = listResult[j];
                    }
                }
            }
         //  RemoveList(listResult);
        }


        /// <summary>
        /// 用分数排除
        /// </summary>
        /// <param name="listResult"></param>
        void ByOCRSource(List<OCRTrain> listResult,float score)
        {
            for (int i = 0; i < listResult.Count; i++)
            {
               if( listResult[i].fScore< score)
                {
                    listResult.RemoveAt(i);
                    ByOCRSource(listResult, score);
                    return;
                }
                
            }
        }

        void RemoveList(List<OCRTrain> listResult)
        {
            for (int i = 0; i < listResult.Count; i++)
            {
                OCRTrain foundi = listResult[i];
                for (int j = i + 1; j < listResult.Count; j++)
                {
                    OCRTrain foundj = listResult[j];
                    if (foundi.strValue == foundj.strValue)
                    {
                        if (foundi.fScore > foundj.fScore)
                            listResult.RemoveAt(j);
                        else
                            listResult.RemoveAt(i);

                        RemoveList(listResult);
                        return;
                    }
                }
            }
        }
#if(AUVISION)
        /// <summary>
        /// 算出偏移及旋轉值
        /// </summary>
        /// <param name="result"></param>
        /// <param name="imginput24"></param>
        /// <param name="imgoutput24"></param>
        public void ScaleRotate(xFindResult result, AUColorImg24 imginput24, ref AUColorImg24 imgoutput24)
        {
            #region 带Mask功能的
            //float fTargetCX = imginput24.GetWidth() / 2.0f;
            //float fTargetCY = imginput24.GetHeight() / 2.0f;
            //float fAffineCX = imginput24.GetWidth() / 2.0f;
            //float fAffineCY = imginput24.GetHeight() / 2.0f;

            //float fCosSida = (float)Math.Cos(-result.fAngle * Math.PI / 180.0f);
            //float fSinSida = (float)Math.Sin(-result.fAngle * Math.PI / 180.0f);
            //float fX = (result.fCenterX - fTargetCX) * fCosSida - (result.fCenterY - fTargetCY) * fSinSida;
            //float fY = (result.fCenterX - fTargetCX) * fSinSida + (result.fCenterY - fTargetCY) * fCosSida;

            ////eInterpolationBits_1,4,8 8 for best but slowest
            //AUImage.ScaleRotate(imginput24, imgoutput24,
            //         fTargetCX, fTargetCY,
            //        //   result.fCenterX, result.fCenterY,
            //        fAffineCX - fX, fAffineCY - fY,
            //        result.fAngle, 1.0f, 1.0f, eInterpolationBits.eInterpolationBits_8);
            #endregion

            AUImage.ScaleRotate(imginput24, imgoutput24,
                                result.fCenterX, result.fCenterY,
                                imgoutput24.GetWidth() / 2, imgoutput24.GetHeight() / 2,
                                result.fAngle, 1.0f, 1.0f, eInterpolationBits.eInterpolationBits_8);

        }
        /// <summary>
        /// 算出偏移及旋轉值
        /// </summary>
        /// <param name="result"></param>
        /// <param name="imginput24"></param>
        /// <param name="imgoutput24"></param>
        public void ScaleRotate(xMatchingResult result, AUColorImg24 imginput24, ref AUColorImg24 imgoutput24)
        {

            AUImage.ScaleRotate(imginput24, imgoutput24,
                                result.fCenterX, result.fCenterY,
                                imgoutput24.GetWidth() / 2, imgoutput24.GetHeight() / 2,
                                result.fAngle, 1.0f, 1.0f, eInterpolationBits.eInterpolationBits_8);

        }
#endif

        /// <summary>
        /// OCR 用AspriseOcr
        /// </summary>
        /// <param name="bmpOcr">源图片</param>
        /// <returns>OCR结果</returns>
        string aOCR(Bitmap bmpOcr)
        {
            try
            {
                string file = "D:\\Jeteazy\\OCR\\ocr.png"; // ☜ jpg, gif, tif, pdf, etc.
                bmpOcr.Save(file,ImageFormat.Png);
                //com_asprise_ocr_setup(0);
                //IntPtr _handle = com_asprise_ocr_start("eng", "fastest");
                //IntPtr ptr = com_asprise_ocr_recognize(_handle.ToInt64(), strMess, -1, -1, -1, -1, -1, "all", "text", "", "|", "=");
                //string s = Marshal.PtrToStringAnsi(ptr).Replace("\n", "");
                //com_asprise_ocr_stop(_handle.ToInt64());
                //com_asprise_ocr_util_delete(_handle.ToInt64(), true);
                asprise_ocr_api.AspriseOCR.SetUp();
                asprise_ocr_api.AspriseOCR.InputLicense("123456", "123456789123456789123456789");
                asprise_ocr_api.AspriseOCR ocr = new asprise_ocr_api.AspriseOCR();
                ocr.StartEngine("eng", asprise_ocr_api.AspriseOCR.SPEED_FASTEST);
                
                //IntPtr strmess = NoParOCR. OCR(file, -1);
                //return Marshal.PtrToStringAnsi(strmess);

                 string s = ocr.Recognize(file, -1, -1, -1, -1, -1, asprise_ocr_api.AspriseOCR.RECOGNIZE_TYPE_TEXT, asprise_ocr_api.AspriseOCR.OUTPUT_FORMAT_PLAINTEXT);
                
                return s;
            }
            catch(Exception ex)
            {
                JetEazy.LoggerClass.Instance.WriteException(ex);
                return "";
            }


        }

        #region OCR参数
        /// <summary>
        /// 用Jet的OCR识别
        /// </summary>
        public bool isJetOCR = true;
        /// <summary>
        /// 用无参数的OCR
        /// </summary>
        public bool isNoParOCR = false;
        /// <summary>
        /// 充许色差
        /// </summary>
        public int iColorDifference = 50;
        /// <summary>
        /// 允许的差异百分比(A-B时，差异的面积与总面积的比值)
        /// （0.00~1.00)
        /// </summary>
        public float fDifference = 0.1f;
        /// <summary>
        ///  排除分数
        /// </summary>
        public float fExcludeScore = 0.01f;
        /// <summary>
        /// 图像拆分亮度
        /// </summary>
        public int iBright=0;
        /// <summary>
        /// 图像拆分比对度
        /// </summary>
        public int iContrast=0;
        /// <summary>
        /// 是否检测缺失
        /// </summary>
        public bool isDefect = false;
        /// <summary>
        /// 点数
        /// </summary>
        public int  iPoint = 3;
        /// <summary>
        /// 面积
        /// </summary>
        public int iArea = 5;
        
        #endregion


    }

    public class NoParOCR
    {
        [DllImport("AspriseOCR.dll", EntryPoint = "OCR", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr OCR(string file, int type);
        [DllImport("AspriseOCR.dll", EntryPoint = "OCRpart", CallingConvention = CallingConvention.Cdecl)]
        static extern IntPtr OCRpart(string file, int type, int startX, int startY, int width, int height);
        [DllImport("AspriseOCR.dll", EntryPoint = "OCRBarCodes", CallingConvention = CallingConvention.Cdecl)]
        static extern IntPtr OCRBarCodes(string file, int type);
        [DllImport("AspriseOCR.dll", EntryPoint = "OCRpartBarCodes", CallingConvention = CallingConvention.Cdecl)]
        static extern IntPtr OCRpartBarCodes(string file, int type, int startX, int startY, int width, int height);

        private const string OCR_DLL_NAME_32 = "aocr_x64.dll";
        [DllImport(OCR_DLL_NAME_32, CharSet = CharSet.Ansi)]
        public static extern int com_asprise_ocr_setup(int queryOnly);
        [DllImport(OCR_DLL_NAME_32, CharSet = CharSet.Ansi)]
        public static extern IntPtr com_asprise_ocr_start(string lang, string speed);
        [DllImport(OCR_DLL_NAME_32, EntryPoint = "com_asprise_ocr_recognize")]
        public static extern IntPtr com_asprise_ocr_recognize(Int64 handle, string imgFiles, int pageIndex, int startX, int startY, int width, int height, string recognizeType, string outputFormat, string propSpec, string propSeparator, string propKeyValueSpeparator);
        [DllImport(OCR_DLL_NAME_32, CharSet = CharSet.Ansi)]
        public static extern void com_asprise_ocr_util_delete(Int64 handle, bool isArray);
        [DllImport(OCR_DLL_NAME_32, CharSet = CharSet.Ansi)]
        public static extern void com_asprise_ocr_stop(Int64 handle);
    }
}
