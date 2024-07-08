using System;
using System.Diagnostics;
using System.IO;
//using Newtonsoft.Json;

using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Net;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Net.Http;
using System.Net.Http.Headers;

namespace JetEazy.BasicSpace
{
    public enum HiveCMD
    {
        ERRORDATA,
        MACHINEDATA,
        MACHINESTATE,

    }
    public class JzHiveItemMessageClass
    {
        public string unit_sn = "";
        public string serials = "";
        public bool ispass = true;
        public DateTime input_time = DateTime.Now;
        public DateTime output_time = DateTime.Now;
        public string eVer = "";
        public string eArtWorkName = "";
        public string eColor = "";

        public string machineName = "";
        public string machineID = "";
        public string KBCountryCode = "";
        public string TestTime = "";

        public string format01Head = "";
        public string format01Value = "";


    }
    public class JzHiveClass
    {
        bool m_IsDebug = false;
        public static string HiveVersion = "Hive5.5_20200601_1";
        private string _hiveclient_path = "c:\\hive\\hiveclient\\hiveclient.exe";
        private string _cmdtxt = "INIT";
        private string _result = "";
        private string _errormsg = "";
        private string _publisher_id_or_machine_id = "9eefa293-7f48-41ed-bb9c-e0700a9e3c52";
        private int _previousstate = 1;
        private string _model = "";

        private const string timeFromat = "yyyy-MM-dd HH:mm:ss.fff";
        private const string timeFromatGTM = "yyyy-MM-ddTHH:mm:ss.fff+0800";
        const int threadCount = 20;//设置线程数量
        int threadIndex = 0;
        /// <summary>
        /// 本机系统上传Hive 是:true 否:false
        /// </summary>
        private bool _islocalsystemupload = true;

        private string _pathHiveTemp = @"D:\hive\JZTemp\";
        private string _pathHive4XX = @"D:\hive\4XX\";
        private string _pathHive5XXOther = @"D:\hive\Other\";

        public JzHiveClass()
        {
            _create_hiveclient_dir();
        }
        public JzHiveClass(string hiveclientpath)
        {
            _hiveclient_path = hiveclientpath;
            _create_hiveclient_dir();
        }
        //public static string HiveVersion
        //{
        //    get { return _hivever; }
        //}

        public bool IsLocalSystemUpload
        {
            get { return _islocalsystemupload; }
            set { _islocalsystemupload = value; }
        }

        public string Model
        {
            get { return _model; }
            set { _model = value; }
        }
        public string HiveclientPath
        {
            get { return _hiveclient_path; }
            set { _hiveclient_path = value; }
        }
        public string PublisherIDOrMachineID
        {
            get { return _publisher_id_or_machine_id; }
            set { _publisher_id_or_machine_id = value; }
        }
        public string CommandText
        {
            get { return _cmdtxt; }
            set { _cmdtxt = value; }
        }
        public string Result
        {
            get { return _result; }
            set { _result = value; }
        }
        public string ErrorMessage
        {
            get { return _errormsg; }
            set { _errormsg = value; }
        }
        public int Execute()
        {
            _result = "";
            _errormsg = "";

            return _command_Hiveclient(_cmdtxt, ref _result, ref _errormsg);
        }
        public int Execute(string strCmd)
        {
            _cmdtxt = strCmd;
            return Execute();
        }

        public int Hiveclient_Init(string site,string building,string line_type,string line,string station_type,string station_instance,string vendor)
        {
            /*
            _cmdtxt = "INIT ";

            _cmdtxt += "-publisher_id=\"" + _publisher_id_or_machine_id + "\" ";
            _cmdtxt += "-site=\"" + site + "\" ";
            _cmdtxt += "-building=\"" + building + "\" ";
            _cmdtxt += "-line_type=\"" + line_type + "\" ";
            _cmdtxt += "-line=\"" + line + "\" ";
            _cmdtxt += "-station_type=\"" + station_type + "\" ";
            _cmdtxt += "-station_instance=\"" + station_instance + "\" ";
            _cmdtxt += "-vendor=\"" + vendor + "\" ";

            return Execute();
            */
            return 0;
        }
        /// <summary>
        /// 發送數據給HIVE
        /// </summary>
        /// <param name="unit_sn"></param>
        /// <param name="serials"></param>
        /// <param name="ispass">測試結果pass:true;fail:false</param>
        /// <returns></returns>
        public int Hiveclient_MachineData(string unit_sn, string serials, bool ispass)
        {
            _cmdtxt = "LOGDATA -table MACHINEDATA ";
            _cmdtxt += "-reqd ";

            //Hashtable hash = new Hashtable();
            //hash.Add("sn1", serials);
            //hash.Add("sn2", serials);
            //string json = JsonConvert.SerializeObject(hash);//{"key1":"val1","key2":"val2"}

            _cmdtxt += "serials=\"{\\\"sn1\\\":\\\"" + serials + "\\\"}\" ";//JSON
            //_cmdtxt += "serials=\"" + json + "\" ";//JSON
            //_cmdtxt += "serials=\"" + serials + "\" ";//JSON
            _cmdtxt += "pass=\"" + (ispass ? "True" : "False") + "\" ";
            _cmdtxt += "unit_sn=\"" + unit_sn + "\" ";
            _cmdtxt += "input_time=\"" + DateTime.Now.ToString(timeFromat) + "\" ";
            _cmdtxt += "output_time=\"" + DateTime.Now.AddSeconds(13d).ToString(timeFromat) + "\" ";
            _cmdtxt += "machine_id=\"" + _publisher_id_or_machine_id + "\" ";
            _cmdtxt += "-data ";
            _cmdtxt += "\"ocr_result\"=\"0\" ";

            return Execute();
        }
        /// <summary>
        /// 發送數據給HIVE
        /// </summary>
        /// <param name="unit_sn"></param>
        /// <param name="serials"></param>
        /// <param name="ispass">測試結果pass:true;fail:false</param>
        /// <returns></returns>
        public int Hiveclient_MachineData(string unit_sn, string serials, bool ispass,DateTime input_time,DateTime output_time)
        {
            _cmdtxt = "LOGDATA -table MACHINEDATA ";
            _cmdtxt += "-reqd ";

            //Hashtable hash = new Hashtable();
            //hash.Add("sn1", serials);
            //hash.Add("sn2", serials);
            //string json = JsonConvert.SerializeObject(hash);//{"key1":"val1","key2":"val2"}

            _cmdtxt += "serials=\"{\\\"sn1\\\":\\\"" + serials + "\\\"}\" ";//JSON
            //_cmdtxt += "serials=\"" + json + "\" ";//JSON
            //_cmdtxt += "serials=\"" + serials + "\" ";//JSON
            _cmdtxt += "pass=\"" + (ispass ? "True" : "False") + "\" ";
            _cmdtxt += "unit_sn=\"" + unit_sn + "\" ";
            _cmdtxt += "input_time=\"" + input_time.ToString(timeFromat) + "\" ";
            _cmdtxt += "output_time=\"" + output_time.ToString(timeFromat) + "\" ";
            _cmdtxt += "machine_id=\"" + _publisher_id_or_machine_id + "\" ";
            _cmdtxt += "-data ";
            _cmdtxt += "\"ocr_result\"=\"0\" ";

            return Execute();
        }
        /// <summary>
        /// 發送數據給HIVE
        /// </summary>
        /// <param name="unit_sn"></param>
        /// <param name="serials"></param>
        /// <param name="ispass">測試結果pass:true;fail:false</param>
        /// <returns></returns>
        public int Hiveclient_MachineData(string unit_sn, string serials, bool ispass, DateTime input_time, DateTime output_time,string strdatajson)
        {
            return 0;
            string url = "http://10.0.0.2:5008/v5/capture/machinedata";
            string data = "";
            TimeSpan ts = output_time - input_time;

            JObject jb0 = new JObject();

            JObject jb01 = new JObject();
            JObject jb02 = new JObject();

            jb0.Add("pass", (ispass ? "True" : "False"));

            jb02.Add("top_case", "");
            jb02.Add("bottom_case", "");
            jb0.Add("serials", jb02);

            jb0.Add("unit_sn", unit_sn);

            jb0.Add("input_time", input_time.ToString(timeFromatGTM));
            jb0.Add("output_time", output_time.ToString(timeFromatGTM));

            //jb01.Add("MeasurementA", "0.55");
            //jb01.Add("MeasurementB", "0.55");
            //jb01.Add("MeasurementC", "0.55");
            //jb01.Add("Fixture No.", "KBOCR01");
            jb01.Add("SN", unit_sn);
            //jb01.Add("Config", "");
            jb01.Add("CT(s)", ts.TotalSeconds.ToString("0.000"));

            //jb01.Add("Offset_11BP_BeforeAlign", "0.3");
            //jb01.Add("CT(s)", "0.3");
            //jb01.Add("CT(s)", "0.3");
            //jb01.Add("CT(s)", "0.3");

            jb01.Add("Pass/Fail", (ispass ? "True" : "False"));
            //jb01.Add("key", "");

            jb0.Add("data", jb01);

            //{
            //    "unit_sn":	"SN000000001",	
            //    "serials":	{ "top_case":	"AB05684AZ",	
            //        "bottom_case":	"67BA099"       },	
            //     "pass":	"true",		
            //    "input_time":	"2018-01-16T13:59:05.06+0800",		
            //    "output_time":	"2018-01-16T13:59:25.06+0800",	
            //    "data":	{ "MeasurementA":	0.56,			
            //        "MeasurementB":	0.55,			
            //        "MeasurementC":	0.5466      }
            //}

            data = jb0.ToString(Formatting.Indented, null);
            string result = _postToHive(HiveCMD.MACHINEDATA, url, data);

            /*
            _cmdtxt = "LOGDATA -table MACHINEDATA ";
            _cmdtxt += "-reqd ";

            //Hashtable hash = new Hashtable();
            //hash.Add("sn1", serials);
            //hash.Add("sn2", serials);
            //string json = JsonConvert.SerializeObject(hash);//{"key1":"val1","key2":"val2"}

            _cmdtxt += "serials=\"{\\\"sn1\\\":\\\"" + serials + "\\\"}\" ";//JSON
            //_cmdtxt += "serials=\"" + json + "\" ";//JSON
            //_cmdtxt += "serials=\"" + serials + "\" ";//JSON
            _cmdtxt += "pass=\"" + (ispass ? "True" : "False") + "\" ";
            _cmdtxt += "unit_sn=\"" + unit_sn + "\" ";
            _cmdtxt += "input_time=\"" + input_time.ToString(timeFromat) + "\" ";
            _cmdtxt += "output_time=\"" + output_time.ToString(timeFromat) + "\" ";
            _cmdtxt += "machine_id=\"" + _publisher_id_or_machine_id + "\" ";
            _cmdtxt += "-data ";
            _cmdtxt += strdatajson.Replace("{","").Replace("}", "").Replace(",", " ").Replace(":", "=");

            return Execute();
            */
            return 0;
        }
        public int Hiveclient_MachineData_Files(JzHiveItemMessageClass eMsgHiveItem, List<string> eListPathFiles = null)
        {

            string url = "http://10.0.0.2:5008/v5/capture/machinedata";
            string data = "";
            TimeSpan ts = eMsgHiveItem.output_time - eMsgHiveItem.input_time;

            JObject jb0 = new JObject();
            jb0.Add("pass", (eMsgHiveItem.ispass ? "True" : "False"));

            JObject jb02 = new JObject();
            JObject jb01 = new JObject();

            //jb02.Add("top_case", "");
            //jb02.Add("bottom_case", "");
            jb0.Add("serials", jb02);

            jb0.Add("unit_sn", eMsgHiveItem.unit_sn);
            jb0.Add("input_time", eMsgHiveItem.input_time.ToString(timeFromatGTM));
            jb0.Add("output_time", eMsgHiveItem.output_time.ToString(timeFromatGTM));

            
            //jb01.Add("SN", unit_sn);
            //jb01.Add("CT", ts.TotalSeconds.ToString("0.000"));//  jb01.Add("CT(s)", ts.TotalSeconds.ToString("0.000"));
            //jb01.Add("Pass/Fail", (ispass ? "True" : "False"));
            jb01.Add("sw_version", HiveVersion);

            JObject jb03 = new JObject();
            jb03.Add("Ver", eMsgHiveItem.eVer);
            jb03.Add("ArtWorkName", eMsgHiveItem.eArtWorkName);
            jb03.Add("Color", eMsgHiveItem.eColor);

            jb03.Add("MachineName", eMsgHiveItem.machineName);
            jb03.Add("MachineID", eMsgHiveItem.machineID);
            jb03.Add("KBCountryCode", eMsgHiveItem.KBCountryCode);
            jb03.Add("TestTime", eMsgHiveItem.TestTime);


            switch (_model.Trim())
            {
                case "J293":
                case "J313":

                    jb03.Add("top_case", "");
                    jb03.Add("bottom_case", "");
                    jb01.Add("CT", ts.TotalSeconds.ToString("0.000"));

                    break;
                default:
                    jb01.Add("CT(s)", ts.TotalSeconds.ToString("0.000"));
                    break;

            }

            JObject jb05 = new JObject();

            if (!string.IsNullOrEmpty(eMsgHiveItem.format01Head))
            {
                string xhead = eMsgHiveItem.format01Head.Replace(Environment.NewLine, "@").Split('@')[0];
                string xheadusl = eMsgHiveItem.format01Head.Replace(Environment.NewLine, "@").Split('@')[1];
                string xheadlsl = eMsgHiveItem.format01Head.Replace(Environment.NewLine, "@").Split('@')[2];

                string[] _heads = xhead.Split(',');
                string[] _headsusl = xheadusl.Split(',');
                string[] _headslsl = xheadlsl.Split(',');
                string[] _value = eMsgHiveItem.format01Value.Split(',');
                int i = 0;
                foreach (string _headstr in _heads)
                {
                    if (!string.IsNullOrEmpty(_headstr))
                    {
                        if (i == 4 || i == 5 || (i >= 11 && i < _heads.Length - 2))
                        {
                            jb01.Add(_headstr, _value[i]);
                            //switch (_model)
                            //{
                            //    case "J293":
                            //    case "J313":

                            //        if (_headstr == "ShopFloor(1=YES)")
                            //            jb01.Add("ShopFloor", _value[i]);
                            //        else
                            //            jb01.Add(_headstr, _value[i]);

                            //        break;
                            //    default:

                            //        jb01.Add(_headstr, _value[i]);
                            //        break;
                            //}
                        }
                        else
                            jb03.Add(_headstr, _value[i]);

                        if (i > 10)
                        {
                            if (!string.IsNullOrEmpty(_headsusl[i]))
                            {
                                JObject jb0501 = new JObject();
                                jb0501.Add("upper_limit", _headsusl[i]);
                                jb0501.Add("lower_limit", _headslsl[i]);

                                jb05.Add(_headstr, jb0501);
                            }
                        }
                    }
                    i++;
                }
            }

            jb0.Add("data", jb01);
            jb0.Add("limit", jb05);
            jb0.Add("attr", jb03);

            if (eListPathFiles != null)
            {
                JArray ja0 = new JArray();
                foreach (string _path in eListPathFiles)
                {
                    FileInfo _fi = new FileInfo(_path);
                    JObject jb04 = new JObject();
                    if (_fi.Extension == ".jpg" || _fi.Extension == ".bmp" || _fi.Extension == ".png")
                    {
                        jb04.Add("file_name", _fi.Name);
                        jb04.Add("retention_policy", 180);//保存圖片180天
                    }
                    else
                    {
                        jb04.Add("file_name", _fi.Name);//<<去掉扩展名 现场那只程式可能未动 20191231
                    }

                    ja0.Add(jb04);
                }
                jb0.Add("blobs", ja0);
            }

            //{
            //    "unit_sn":	"SN000000001",	
            //    "serials":	{ "top_case":	"AB05684AZ",	
            //        "bottom_case":	"67BA099"       },	
            //     "pass":	"true",		
            //    "input_time":	"2018-01-16T13:59:05.06+0800",		
            //    "output_time":	"2018-01-16T13:59:25.06+0800",	
            //    "data":	{ "MeasurementA":	0.56,			
            //        "MeasurementB":	0.55,			
            //        "MeasurementC":	0.5466      }
            //}

            data = jb0.ToString(Formatting.Indented, null);
            string result = _postToHive(HiveCMD.MACHINEDATA, url, data, eListPathFiles);


            return 0;
        }

        /* 备份之前的资料
         * public int Hiveclient_MachineData_Files(string unit_sn, 
                                                                                   string serials, 
                                                                                   bool ispass, 
                                                                                   DateTime input_time, 
                                                                                   DateTime output_time, 
                                                                                   string eVer,
                                                                                   string eArtWorkName,
                                                                                   string eColor,
                                                                                   List<string> eListPathFiles = null)
        {

            string url = "http://10.0.0.2:5008/v5/capture/machinedata";
            string data = "";
            TimeSpan ts = output_time - input_time;

            JObject jb0 = new JObject();
            jb0.Add("pass", (ispass ? "True" : "False"));

            JObject jb02 = new JObject();
            //jb02.Add("top_case", "");
            //jb02.Add("bottom_case", "");
            jb0.Add("serials", jb02);

            jb0.Add("unit_sn", unit_sn);
            jb0.Add("input_time", input_time.ToString(timeFromatGTM));
            jb0.Add("output_time", output_time.ToString(timeFromatGTM));
            
            JObject jb01 = new JObject();
            //jb01.Add("SN", unit_sn);
            jb01.Add("CT(s)", ts.TotalSeconds.ToString("0.000"));
            //jb01.Add("Pass/Fail", (ispass ? "True" : "False"));
            jb01.Add("sw_version", HiveVersion);
            jb0.Add("data", jb01);

            JObject jb03 = new JObject();
            jb03.Add("Ver", eVer);
            jb03.Add("ArtWorkName", eArtWorkName);
            jb03.Add("Color", eColor);
            jb0.Add("attr", jb03);

            if (eListPathFiles != null)
            {
                JArray ja0 = new JArray();
                foreach (string _path in eListPathFiles)
                {
                    FileInfo _fi = new FileInfo(_path);
                    JObject jb04 = new JObject();
                    if (_fi.Extension == ".jpg" || _fi.Extension == ".bmp" || _fi.Extension == ".png")
                    {
                        jb04.Add("file_name", _fi.Name);
                        jb04.Add("retention_policy", 180);//保存圖片180天
                    }
                    else
                    {
                        jb04.Add("file_name", _fi.Name);//<<去掉扩展名 现场那只程式可能未动 20191231
                    }

                    ja0.Add(jb04);
                }
                jb0.Add("blobs", ja0);
            }

            //{
            //    "unit_sn":	"SN000000001",	
            //    "serials":	{ "top_case":	"AB05684AZ",	
            //        "bottom_case":	"67BA099"       },	
            //     "pass":	"true",		
            //    "input_time":	"2018-01-16T13:59:05.06+0800",		
            //    "output_time":	"2018-01-16T13:59:25.06+0800",	
            //    "data":	{ "MeasurementA":	0.56,			
            //        "MeasurementB":	0.55,			
            //        "MeasurementC":	0.5466      }
            //}

            data = jb0.ToString(Formatting.Indented, null);
            string result = _postToHive(HiveCMD.MACHINEDATA, url, data, eListPathFiles);

            
            return 0;
        }
         * 
        */

        /// <summary>
        /// 發送數據給HIVE,通過文件txt
        /// </summary>
        /// <param name="unit_sn"></param>
        /// <param name="serials"></param>
        /// <param name="ispass"></param>
        /// <param name="m_datafile_path">結果文件路徑</param>
        /// <returns></returns>
        public int Hiveclient_MachineData_Files(string unit_sn, string serials, bool ispass,string m_datafile_path)
        {
            _cmdtxt = "LOGDATA -table MACHINEDATA ";
            _cmdtxt += "-reqd ";

            //Hashtable hash = new Hashtable();
            //hash.Add("sn1", serials);
            //hash.Add("sn2", serials);
            //string json = JsonConvert.SerializeObject(hash);//{"key1":"val1","key2":"val2"}

            _cmdtxt += "serials=\"{\\\"sn1\\\":\\\"" + serials + "\\\"}\" ";//JSON
            //_cmdtxt += "serials=\"" + json + "\" ";//JSON
            //_cmdtxt += "serials=\"" + serials + "\" ";//JSON
            _cmdtxt += "pass=\"" + (ispass ? "True" : "False") + "\" ";
            _cmdtxt += "unit_sn=\"" + unit_sn + "\" ";
            _cmdtxt += "input_time=\"" + DateTime.Now.ToString(timeFromat) + "\" ";
            _cmdtxt += "output_time=\"" + DateTime.Now.AddSeconds(13d).ToString(timeFromat) + "\" ";
            _cmdtxt += "machine_id=\"" + _publisher_id_or_machine_id + "\" ";
            _cmdtxt += "-data ";
            _cmdtxt += "\"@" + m_datafile_path + "\" ";

            return Execute();
        }
        /// <summary>
        /// 發送數據給HIVE,通過文件txt
        /// </summary>
        /// <param name="unit_sn"></param>
        /// <param name="serials"></param>
        /// <param name="ispass"></param>
        /// <param name="m_datafile_path">結果文件路徑</param>
        /// <returns></returns>
        public int Hiveclient_MachineData_Files(string unit_sn, string serials, bool ispass, string m_datafile_path, DateTime input_time, DateTime output_time)
        {
            _cmdtxt = "LOGDATA -table MACHINEDATA ";
            _cmdtxt += "-reqd ";

            //Hashtable hash = new Hashtable();
            //hash.Add("sn1", serials);
            //hash.Add("sn2", serials);
            //string json = JsonConvert.SerializeObject(hash);//{"key1":"val1","key2":"val2"}

            _cmdtxt += "serials=\"{\\\"sn1\\\":\\\"" + serials + "\\\"}\" ";//JSON
            //_cmdtxt += "serials=\"" + json + "\" ";//JSON
            //_cmdtxt += "serials=\"" + serials + "\" ";//JSON
            _cmdtxt += "pass=\"" + (ispass ? "True" : "False") + "\" ";
            _cmdtxt += "unit_sn=\"" + unit_sn + "\" ";
            _cmdtxt += "input_time=\"" + input_time.ToString(timeFromat) + "\" ";
            _cmdtxt += "output_time=\"" + output_time.ToString(timeFromat) + "\" ";
            _cmdtxt += "machine_id=\"" + _publisher_id_or_machine_id + "\" ";
            _cmdtxt += "-data ";
            _cmdtxt += "\"@" + m_datafile_path + "\" ";

            return Execute();
        }
        public int Hiveclient_AlignmentData(string unit_sn, string serials, bool ispass,string step)
        {
            _cmdtxt = "LOGDATA -table ALIGNMENTDATA ";
            _cmdtxt += "-reqd ";
            _cmdtxt += "serials=\"{\\\"sn1\\\":\\\"" + serials + "\\\"}\" ";//JSON
            _cmdtxt += "pass=\"" + (ispass ? "True" : "False") + "\" ";
            _cmdtxt += "unit_sn=\"" + unit_sn + "\" ";
            _cmdtxt += "input_time=\"" + DateTime.Now.ToString(timeFromat) + "\" ";
            //_cmdtxt += "output_time=\"" + DateTime.Now.ToString(timeFromat) + "\" ";
            _cmdtxt += "measurement_step=\"" + step + "\" ";
            _cmdtxt += "alignment_cycles=\"" + "7" + "\" ";
            _cmdtxt += "cycle_time=\"" + "1.2" + "\" ";
            //_cmdtxt += "file_path=\"" + "..." + "\" ";
            _cmdtxt += "machine_id=\"" + _publisher_id_or_machine_id + "\" ";
            _cmdtxt += "-data ";
            _cmdtxt += "\"ocr_result\"=\"0\" ";

            return Execute();
        }
        public int Hiveclient_ErrorData(string err_msg, string err_code, DateTime occurrence_time, DateTime resolved_time)
        {

            string url = "http://10.0.0.2:5008/v5/capture/errordata";
            string data = "";

            JObject jb0 = new JObject();

            JObject jb01 = new JObject();
            JObject jb02 = new JObject();

            jb0.Add("message", err_msg);
            jb0.Add("code", err_code);
            jb0.Add("severity", "warning");
            jb0.Add("occurrence_time", occurrence_time.ToString(timeFromatGTM));
            jb0.Add("resolved_time", resolved_time.ToString(timeFromatGTM));

            jb01.Add("key", "");
            jb0.Add("data", jb01);

            //  {
            //      "message":	"CCD	miscapture",	
            //      "code":	"4500",		
            //      "severity":	"warning",
            //      "occurrence_time":	"2018-01-16T13:59:05.06+0800",		
            //      "resolved_time":	"2018-01-16T13:59:05.06+0800",	
            //      "data":	{ "key":	"example"       }
            //  }

            data = jb0.ToString(Formatting.Indented, null);
            string result = _postToHive(HiveCMD.ERRORDATA, url, data);
            /*
            TimeSpan ts = resolved_time - occurrence_time;

            _cmdtxt = "LOGDATA -table ERRORDATA ";
            _cmdtxt += "-reqd ";
            _cmdtxt += "message=\"" + err_msg + "\" ";
            _cmdtxt += "code=\"" + err_code + "\" ";
            _cmdtxt += "occurrence_time=\"" + occurrence_time.ToString(timeFromat) + "\" ";
            _cmdtxt += "resolved_time=\"" + resolved_time.ToString(timeFromat) + "\" ";
            _cmdtxt += "severity=\"" + "warning" + "\" ";
            _cmdtxt += "machine_id=\"" + _publisher_id_or_machine_id + "\" ";
            _cmdtxt += "-data ";
            _cmdtxt += "\"" + err_msg + "\"=\"" + ts.TotalSeconds.ToString("0.000") + "\" ";

            return Execute();
            */
            return 0;
        }
        /// <summary>
        /// 記錄機台狀態
        /// </summary>
        /// <param name="machine_state">1.Running;2.idle;3.Error;4.engineering mode;5.planned downtime</param>
        /// <param name="state_change_time">記錄改變狀態時間</param>
        /// <param name="previousstate">前一個狀態</param>
        /// <param name="isinit">初始化沒有前一個狀態</param>
        /// <returns>返回0為命令完成,-1為命令失敗</returns>
        public int Hiveclient_MachineState(int machine_state, bool isinit)
        {
            string url = "http://10.0.0.2:5008/v5/capture/machinestate";
            string data = "";

            JObject jb0 = new JObject();

            JObject jb01 = new JObject();
            JObject jb02 = new JObject();
            if (_previousstate != machine_state)
            {
                jb0.Add("machine_state", machine_state.ToString());
                jb0.Add("state_change_time", DateTime.Now.ToString(timeFromatGTM));


                jb01.Add("previous_state", (!isinit ? _previousstate.ToString() : ""));
                jb01.Add("message", "");
                jb01.Add("code", "");
                jb01.Add("occurrence_time", "");

                jb0.Add("data", jb01);
                _previousstate = machine_state;
                //"machine_state":	2,	
                //"state_change_time":	"2018-01-16T13:59:05.06+0800",	
                //"data":	{ }

                data = jb0.ToString(Formatting.Indented, null);
                string result = _postToHive(HiveCMD.MACHINESTATE, url, data);
            }

            /* OLD USE HIVECLENT
            if (_previousstate != machine_state)
            {
                _cmdtxt = "LOGDATA -table MACHINESTATE ";
                _cmdtxt += "-reqd ";
                _cmdtxt += "machine_state=\"" + machine_state + "\" ";
                //if (!isinit)
                _cmdtxt += "state_change_time=\"" + DateTime.Now.ToString(timeFromat) + "\" ";
                _cmdtxt += "machine_id=\"" + _publisher_id_or_machine_id + "\" ";
                _cmdtxt += "-data ";
                if (!isinit)
                    _cmdtxt += "\"" + "Previousstate" + "\"=\"" + _previousstate + "\" ";
                _previousstate = machine_state;
            }
            else
                return 0;

            return Execute();
            */

            return 0;
        }
        /// <summary>
        /// 機台發生錯誤狀態為5
        /// </summary>
        /// <param name="state_change_time">記錄時間</param>
        /// <param name="previousstate">前一狀態</param>
        /// <param name="err_msg">錯誤信息</param>
        /// <param name="err_code">錯誤代碼</param>
        /// <param name="occurrence_time">恢復時間</param>
        /// <returns>返回0為命令完成,-1為命令失敗</returns>
        public int Hiveclient_MachineState_Errmsg(string err_msg, string err_code, DateTime occurrence_time)
        {

            string url = "http://10.0.0.2:5008/v5/capture/machinestate";
            string data = "";

            JObject jb0 = new JObject();

            JObject jb01 = new JObject();
            JObject jb02 = new JObject();
            //if (_previousstate != machine_state)
            {
                jb0.Add("machine_state", "5");
                jb0.Add("state_change_time", DateTime.Now.ToString(timeFromatGTM));

                jb01.Add("previous state", _previousstate.ToString());
                jb01.Add("message", err_msg);
                jb01.Add("code", err_code);
                jb01.Add("occurrence_time", occurrence_time.ToString(timeFromatGTM));

                jb0.Add("data", jb01);
                _previousstate = 5;
                //"machine_state":	2,	
                //"state_change_time":	"2018-01-16T13:59:05.06+0800",	
                //"data":	{ }

                data = jb0.ToString(Formatting.Indented, null);
                string result = _postToHive(HiveCMD.MACHINESTATE, url, data);
            }


            /*
            _cmdtxt = "LOGDATA -table MACHINESTATE ";
            _cmdtxt += "-reqd ";
            _cmdtxt += "machine_state=\"" + "5" + "\" ";
            //if (!isinit)
                _cmdtxt += "state_change_time=\"" + state_change_time.ToString(timeFromat) + "\" ";
            _cmdtxt += "machine_id=\"" + _publisher_id_or_machine_id + "\" ";
            _cmdtxt += "-data ";
            //if (!isinit)
            _cmdtxt += "\"" + "Previousstate" + "\"=\"" + _previousstate + "\" ";

            _cmdtxt += "message=\"" + err_msg + "\" ";
            _cmdtxt += "code=\"" + err_code + "\" ";
            _cmdtxt += "occurrence_time=\"" + occurrence_time.ToString(timeFromat) + "\" ";

            return Execute();
            */
            return 0;
        }
        public int Hiveclient_ConfigurationMap(string unit_sn,string unit_type,string program,string build_config,string strjson)
        {
            /*
            _cmdtxt = "LOGDATA -table CONFIGURATIONMAP ";
            _cmdtxt += "-reqd ";
            _cmdtxt += "unit_sn=\"" + unit_sn + "\" ";
            _cmdtxt += "sn_type=\"" + unit_type + "\" ";
            _cmdtxt += "program=\"" + program + "\" ";
            _cmdtxt += "build_config=\"" + build_config + "\" ";
            _cmdtxt += "machine_id=\"" + _publisher_id_or_machine_id + "\" ";
            _cmdtxt += "-data ";
            //_cmdtxt += "\"" + "mapping_list" + "\":\"[" + strjson + "]\"";

            return Execute();
            */
            return 0;
        }
        public int Hiveclient_Scan(string serials, string unit_sn,DateTime scan_time)
        {
            _cmdtxt = "LOGDATA -table SCAN ";
            _cmdtxt += "-reqd ";
            _cmdtxt += "serials=\"{\\\"sn1\\\":\\\"" + serials + "\\\"}\" ";//JSON
            _cmdtxt += "unit_sn=\"" + unit_sn + "\" ";
            _cmdtxt += "scan_time=\"" + scan_time.ToString(timeFromat) + "\" ";
            _cmdtxt += "machine_id=\"" + _publisher_id_or_machine_id + "\" ";
            _cmdtxt += "-data ";

            return Execute();
        }

        private int _command_Hiveclient(string cmdParameter,ref string displayText,ref string exceptionmsg)
        {
            int bOK = 0;
            displayText = "No Message.";
            exceptionmsg = "";
            //txtResult.Text = string.Empty;
            //if (txtExecutable.Text.Trim() != string.Empty)
            //{
                StreamReader outputReader = null;
                StreamReader errorReader = null;

                try
                {
                    //btnStart.Enabled = false;

                    //Create Process Start information
                    ProcessStartInfo processStartInfo =
                        new ProcessStartInfo(_hiveclient_path, cmdParameter);
                    processStartInfo.WorkingDirectory = Path.GetDirectoryName(_hiveclient_path);

                    _LogAllAction("******************Begin******************" + Environment.NewLine);
                    _LogAllAction("Process.workingDirectory," + processStartInfo.WorkingDirectory);
                    _LogAllAction("cmd:" + cmdParameter);

                    processStartInfo.ErrorDialog = false;
                    processStartInfo.UseShellExecute = false;
                    processStartInfo.RedirectStandardError = true;
                    processStartInfo.RedirectStandardInput = true;
                    processStartInfo.RedirectStandardOutput = true;
                    //设置cmd窗口不显示
                    processStartInfo.CreateNoWindow = true;

                    //Execute the process
                    Process process = new Process();
                    process.StartInfo = processStartInfo;
                    bool processStarted = process.Start();
                    if (processStarted)
                    {
                        //Get the output stream
                        outputReader = process.StandardOutput;
                        errorReader = process.StandardError;
                        process.WaitForExit();

                        //Display the result
                        //displayText = Environment.NewLine;
                        //displayText += Environment.NewLine;
                        //displayText += "******************Begin******************" + Environment.NewLine;
                        displayText = "Output" + Environment.NewLine;
                        displayText += "_____________________" + Environment.NewLine;
                        displayText += outputReader.ReadToEnd();
                        displayText += "Error" + Environment.NewLine;
                        displayText += "_____________________" + Environment.NewLine;
                        displayText += errorReader.ReadToEnd();
                        //displayText += "******************End******************" + Environment.NewLine;
                        displayText += Environment.NewLine;
                        displayText += Environment.NewLine;
                        //txtResult.Text = displayText;

                        _LogAllAction("Received:" + Environment.NewLine + displayText);
                        _LogAllAction("******************End******************" + Environment.NewLine);
                    }
                }
                catch (Exception ex)
            {
                JetEazy.LoggerClass.Instance.WriteException(ex);
                bOK = -1;
                    exceptionmsg = ex.Message;
                    _LogAllAction("JzHive.Exception:" + Environment.NewLine + exceptionmsg);
                    //MessageBox.Show(ex.Message);
                }
                finally
                {
                    if (outputReader != null)
                    {
                        outputReader.Close();
                    }
                    if (errorReader != null)
                    {
                        errorReader.Close();
                    }
                    //btnStart.Enabled = true;
                }
            //}
            //else
            //{
            //    //MessageBox.Show("Please select executable.");
            //}

            return bOK;
        }
        private void _LogAllAction(string strMsg)
        {
            //lock (LAST_RECIPE_NAME)
            //{
            string strPath = "D:\\log\\log.hiveclient\\" + DateTime.Now.ToString("yyyyMMdd") + "\\";
            if (!System.IO.Directory.Exists(strPath))
                System.IO.Directory.CreateDirectory(strPath);

            string strFileName = strPath + DateTime.Now.ToString("yyyyMMdd_HH") + ".log";
            System.IO.StreamWriter stm = null;

            try
            {
                stm = new System.IO.StreamWriter(strFileName, true, System.Text.Encoding.Default);
                stm.Write(DateTime.Now.ToString("HH:mm:ss"));
                stm.Write(",hiveclient");
                stm.Write(", ");
                stm.WriteLine(strMsg);
                stm.Flush();
                stm.Close();
                stm.Dispose();
                stm = null;
            }
            catch(Exception ex)
            {
                JetEazy.LoggerClass.Instance.WriteException(ex);
            }

            if (stm != null)
                stm.Dispose();
            //}
        }
        private void _SaveOverData(string _pathname, string datas)
        {
            string strFileName = _pathname;
            System.IO.StreamWriter stm = null;

            try
            {
                stm = new System.IO.StreamWriter(strFileName, true, System.Text.Encoding.UTF8);
                stm.Write(datas);
                stm.Flush();
                stm.Close();
                stm.Dispose();
                stm = null;
            }
            catch(Exception ex)
            {
                JetEazy.LoggerClass.Instance.WriteException(ex);
            }

            if (stm != null)
                stm.Dispose();
        }
        private void _LogAllAction(string dir,string strMsg)
        {
            //lock (LAST_RECIPE_NAME)
            //{
            string strPath = "D:\\log\\log.hiveclient\\" + DateTime.Now.ToString("yyyyMMdd") + "\\"+ dir +"\\";
            if (!System.IO.Directory.Exists(strPath))
                System.IO.Directory.CreateDirectory(strPath);

            string strFileName = strPath + DateTime.Now.ToString("yyyyMMdd_HH") + ".log";
            System.IO.StreamWriter stm = null;

            try
            {
                stm = new System.IO.StreamWriter(strFileName, true, System.Text.Encoding.Default);
                stm.Write(DateTime.Now.ToString("HH:mm:ss"));
                stm.Write(",");
                stm.Write(JzHiveClass.HiveVersion);
                stm.Write(",");
                stm.WriteLine(strMsg);
                stm.Flush();
                stm.Close();
                stm.Dispose();
                stm = null;
            }
            catch(Exception ex)
            {
                JetEazy.LoggerClass.Instance.WriteException(ex);
            }

            if (stm != null)
                stm.Dispose();
            //}
        }
        /// <summary>
        /// 检查必要的路径
        /// </summary>
        private void _create_hiveclient_dir()
        {
            string strconfigpath = @"C:\hive\hiveclient\config\";
            string strlogpath = @"C:\hive\hiveclient\log\";
            string stroutpath = @"C:\hive\hiveclient\out\";
            string strremotedatapath = @"C:\hive\hiveclient\remotedata\";
            string strremoteerrorpath = @"C:\hive\hiveclient\remoteerror\";
            string strskynetpath = @"D:\SkynetData\";
            string strinspectdatapath = @"D:\Inspectdata\";
            //这些是apple hiveclient 需要的路径 那个不会自己生成 所以我们帮他一下
            if (!Directory.Exists(strconfigpath))
                Directory.CreateDirectory(strconfigpath);
            if (!Directory.Exists(strlogpath))
                Directory.CreateDirectory(strlogpath);
            if (!Directory.Exists(stroutpath))
                Directory.CreateDirectory(stroutpath);
            if (!Directory.Exists(strremotedatapath))
                Directory.CreateDirectory(strremotedatapath);
            if (!Directory.Exists(strremoteerrorpath))
                Directory.CreateDirectory(strremoteerrorpath);
            if (!Directory.Exists(strskynetpath))
                Directory.CreateDirectory(strskynetpath);
            if (!Directory.Exists(strinspectdatapath))
                Directory.CreateDirectory(strinspectdatapath);

            if (!Directory.Exists(_pathHiveTemp))
                Directory.CreateDirectory(_pathHiveTemp);
            if (!Directory.Exists(_pathHive4XX))
                Directory.CreateDirectory(_pathHive4XX);
            if (!Directory.Exists(_pathHive5XXOther))
                Directory.CreateDirectory(_pathHive5XXOther);
        }

        private string _postToHive(HiveCMD _cmd, string Url, string datas, List<string> listfilepath = null)
        {

            if (threadIndex >= threadCount || !_islocalsystemupload)
            {
                string _OvertTheadFileName = "ORG_" + _cmd.ToString() + "_" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + ".txt";

                switch (_cmd)
                {
                    case HiveCMD.ERRORDATA:
                    case HiveCMD.MACHINESTATE:

                        _SaveOverData(_pathHiveTemp + _OvertTheadFileName, datas);

                        break;
                    case HiveCMD.MACHINEDATA:

                        if (listfilepath == null)
                        {
                            _SaveOverData(_pathHiveTemp + _OvertTheadFileName, datas);
                        }
                        else
                        {
                            datas = "jsondata=" + datas + Environment.NewLine;
                            int i = 0;
                            foreach (string _path in listfilepath)
                            {
                                datas += "file" + i.ToString() + "=" + _path + Environment.NewLine;
                                i++;
                            }
                            _OvertTheadFileName = "Files_" + _cmd.ToString() + "_" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + ".txt";
                            _SaveOverData(_pathHiveTemp + _OvertTheadFileName, datas);
                        }

                        break;
                }
                return "OverThreadCount";
            }

            if (m_IsDebug)
                Url = "http://httpbin.org/anything";

            HiveItem _hiveitem = null;
            if (listfilepath == null)
            {
                _hiveitem = new HiveItem(_cmd, Url, datas);

                System.Threading.Thread m_thread_postHive = new System.Threading.Thread(new System.Threading.ParameterizedThreadStart(_ThreadPostHive));
                m_thread_postHive.IsBackground = true;
                m_thread_postHive.Start(_hiveitem);
            }
            else
            {
                _hiveitem = new HiveItem(_cmd, Url, datas, listfilepath);

                System.Threading.Thread m_thread_postHive = new System.Threading.Thread(new System.Threading.ParameterizedThreadStart(_ThreadPostHiveList));
                m_thread_postHive.IsBackground = true;
                m_thread_postHive.Start(_hiveitem);
            }
                

            //HiveItem _hiveitem = new HiveItem(_cmd, Url, datas);

            threadIndex++;

            return "";
        }

        private void _ThreadPostHive(object o)
        {
            HiveItem _hiveitem = o as HiveItem;

            HiveCMD _cmd = _hiveitem.m_hivecmd;
            string Url = _hiveitem.m_url;
            string datas = _hiveitem.m_datas;

            DateTime dtStart = DateTime.Now;
            DateTime dtEnd = DateTime.Now;

            StringBuilder content = new StringBuilder();
            HttpWebResponse response = null;
            //_LogAllAction(_cmd.ToString(), "******************Begin******************" + Environment.NewLine);
            //_LogAllAction(_cmd.ToString(), "Post data:\r\n" + datas);
            try
            {
                // 与指定URL创建HTTP请求  
                System.Net.HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Url);
                //request.UserAgent = "Mozilla/5.0 (compatible; MSIE 9.0; Windows NT 6.1; Trident/5.0; BOIE9;ZHCN)";
                request.Method = "POST";
                request.Accept = "application/json";
                request.ContentType = "application/json";
                request.ServicePoint.Expect100Continue = true;
                request.Proxy = null;

                byte[] datasByte = Encoding.Default.GetBytes(datas);
                request.ContentLength = datasByte.Length;
                //不保持连接  
                //request.KeepAlive = false;
                Stream requestStream = request.GetRequestStream();
                requestStream.Write(datasByte, 0, datasByte.Length);
                requestStream.Close();

                // 获取对应HTTP请求的响应  
                response = (HttpWebResponse)request.GetResponse();

                // 获取响应流  
                Stream responseStream = response.GetResponseStream();

                // 对接响应流(以"GBK"字符集)  
                StreamReader sReader = new StreamReader(responseStream, Encoding.UTF8);
                // 开始读取数据  
                Char[] sReaderBuffer = new Char[256];
                int count = sReader.Read(sReaderBuffer, 0, 256);
                while (count > 0)
                {
                    String tempStr = new String(sReaderBuffer, 0, count);
                    content.Append(tempStr);
                    count = sReader.Read(sReaderBuffer, 0, 256);
                }
                // 读取结束  
                sReader.Close();

                //if (content.ToString() == "")
                //    return "NULL";
            }
            catch (Exception ex)
            {
                JetEazy.LoggerClass.Instance.WriteException(ex);
                content = new StringBuilder("Runtime Error\r\n" + ex.Message);

                //if (threadIndex >= threadCount)
                {
                    string _OvertTheadFileName = "Exception_" + _cmd.ToString() + "_" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + ".txt";
                    _SaveOverData(_pathHiveTemp + _OvertTheadFileName, datas);

                    //return "OverThreadCount";
                }
            }

            dtEnd = DateTime.Now;
            TimeSpan ts = dtEnd - dtStart;

            if (response != null)
                response.Close();

            _LogAllAction(_cmd.ToString(), "******************Begin******************  StartTime " + dtStart.ToString("yyyy/MM/dd HH:mm:ss.fff") + Environment.NewLine);
            _LogAllAction(_cmd.ToString(), "Post data:\r\n" + datas);

            _LogAllAction(_cmd.ToString(), "Elapsed time(s):" + ts.TotalSeconds.ToString("0.000") + " Return:\r\n" + content.ToString());
            _LogAllAction(_cmd.ToString(), "******************End********************  EndTime " + dtEnd.ToString("yyyy/MM/dd HH:mm:ss.fff") + Environment.NewLine);

            if (threadIndex >= 20)
                threadIndex--;

            //return content.ToString();
        }
        private void _ThreadPostHiveList(object o)
        {
            HiveItem _hiveitem = o as HiveItem;

            HiveCMD _cmd = _hiveitem.m_hivecmd;
            string Url = _hiveitem.m_url;
            string datas = _hiveitem.m_datas;
            List<string> filepath = _hiveitem.m_list_filepath;

            string str_request = "";

            DateTime dtStart = DateTime.Now;
            DateTime dtEnd = DateTime.Now;

            var client = new HttpClient();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("*/*"));
            client.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("curl", "7.55.1"));
            MultipartFormDataContent requestContent = new MultipartFormDataContent();

            var contentstring = new StringContent(datas);
            requestContent.Add(contentstring, "jsondata");

            str_request += "jsondata=" + datas + Environment.NewLine;


            int i = 0;
            string data = "";

            try
            {

                foreach (string _path in filepath)
                {
                    FileInfo _fi = new FileInfo(_path);
                    requestContent.Add(new ByteArrayContent(File.ReadAllBytes(_path)), _fi.Name, _fi.Name);
                    //requestContent.Add(new ByteArrayContent(File.ReadAllBytes(_path)), _fi.Name);//<<USE

                    str_request += "file" + i.ToString() + "=" + _path + Environment.NewLine;

                    i++;
                }

                // response
                var response = client.PostAsync(Url, requestContent).Result;
                data = response.Content.ReadAsStringAsync().Result;
            }
            catch (Exception ex)
            {
                data += "Exception:" + Environment.NewLine;
                data += ex.ToString();
                JetEazy.LoggerClass.Instance.WriteException(ex);
                //string _OvertTheadFileName = "Exception_" + _cmd.ToString() + "_" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + ".txt";
                //if (filepath == null)
                //{
                //    _SaveOverData(_pathHiveTemp + _OvertTheadFileName, datas);
                //}
                //else
                //{
                //    //datas = "jsondata=" + datas + Environment.NewLine;
                //    //i = 0;
                //    //foreach (string _path in filepath)
                //    //{
                //    //    datas += "file" + i.ToString() + "=" + _path + Environment.NewLine;
                //    //    i++;
                //    //}
                //    _OvertTheadFileName = "ExceptionFiles_" + _cmd.ToString() + "_" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + ".txt";
                //    _SaveOverData(_pathHiveTemp + _OvertTheadFileName, str_request);
                //}
            }

            //MessageBox.Show(data);

            dtEnd = DateTime.Now;
            TimeSpan ts = dtEnd - dtStart;

            if (data.IndexOf("\"ErrorCode\":null,\"ErrorText\":null,\"Status\":\"Success") > -1)
            {

            }
            else
            {
                string _OvertTheadFileName = "Exception_" + _cmd.ToString() + "_" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + ".txt";
                if (filepath == null)
                {
                    _SaveOverData(_pathHiveTemp + _OvertTheadFileName, datas);
                }
                else
                {
                    _OvertTheadFileName = "ExceptionFiles_" + _cmd.ToString() + "_" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + ".txt";
                    _SaveOverData(_pathHiveTemp + _OvertTheadFileName, str_request);
                }
            }

            //_LogAllAction(_cmd.ToString(), "******************Begin******************  StartTime " + dtStart.ToString("yyyy/MM/dd HH:mm:ss.fff") + Environment.NewLine);
            //_LogAllAction(_cmd.ToString(), "Post data:\r\n" + str_request.ToString());

            //_LogAllAction(_cmd.ToString(), "Elapsed time(s):" + ts.TotalSeconds.ToString("0.000") + " Return:\r\n" + data.ToString());
            //_LogAllAction(_cmd.ToString(), "******************End********************  EndTime " + dtEnd.ToString("yyyy/MM/dd HH:mm:ss.fff") + Environment.NewLine);

            string message = "";

            message += Environment.NewLine;
            message += _cmd.ToString() + Environment.NewLine;
            message += "******************Begin******************  StartTime " + dtStart.ToString("yyyy/MM/dd HH:mm:ss.fff") + Environment.NewLine;

            message += "Post data:" + Environment.NewLine;
            message += str_request.ToString() + Environment.NewLine;

            message += "Elapsed time(s):" + ts.TotalSeconds.ToString("0.000") + Environment.NewLine;
            message += "Return:" + Environment.NewLine;
            message += data.ToString() + Environment.NewLine;

            message += "******************End********************  EndTime " + dtEnd.ToString("yyyy/MM/dd HH:mm:ss.fff") + Environment.NewLine;

            _LogAllAction(_cmd.ToString(), message);

            //if (m_rtb_message != null)
            //{
            //    m_rtb_message.Invoke(new Action(() =>
            //    {
            //        m_rtb_message.Text = message;
            //    }));
            //}

            if (threadIndex >= 20)
                threadIndex--;

            //return content.ToString();
        }
    }

    public class HiveItem
    {
        const char SeperateCharA = '\x1e';

        public HiveCMD m_hivecmd = HiveCMD.MACHINESTATE;
        public string m_url = "http://10.0.0.2:5008/v5/capture/machinestate";
        public string m_datas = "";
        public List<string> m_list_filepath = new List<string>();
        public HiveItem(HiveCMD eHiveCMD,string eUrl,string eDatas)
        {
            m_hivecmd = eHiveCMD;
            m_url = eUrl;
            m_datas = eDatas;
        }
        public HiveItem(HiveCMD eHiveCMD, string eUrl, string eDatas, List<string> eListFilePath)
        {
            m_hivecmd = eHiveCMD;
            m_url = eUrl;
            m_datas = eDatas;
            m_list_filepath = eListFilePath;
        }
        public object ToHiveItem()
        {
            object o = new object();

            o += m_hivecmd.ToString() + SeperateCharA;
            o += m_url + SeperateCharA;
            o += m_datas;

            return o;
        }

        public void Froming(object o)
        {
            string str = o as string;
            string[] strs = str.Split(SeperateCharA);
            m_hivecmd = (HiveCMD)int.Parse(strs[0]);
            m_url = strs[1];
            m_datas = strs[2];

        }

    }
}
