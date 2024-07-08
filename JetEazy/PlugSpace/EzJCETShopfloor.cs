using JetEazy.BasicSpace;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Net;
using System.Xml;

namespace JetEazy.PlugSpace
{
    internal class EzJCETShopfloor
    {
    }
    public interface IShopfloor
    {
        bool GetRecipeFromLot(string strlot, string URL);
        string GetVersion();
        int GetMESStrip();
        string GetShowMessage();
    }

    public class EzShopfloorClass : IShopfloor
    {
        private string m_recipe_version = "TST";
        private int m_mes_strip = 0;
        //private JcetService.InkMarking webservice;
        //private JcetService.Service1 webservice;
        private string m_show_message = "";

        public EzShopfloorClass()
        {
            //webservice = new JcetService.InkMarking();
            //webservice.UnsafeAuthenticatedConnectionSharing = true;
            //webservice.AllowAutoRedirect = true;
            //webservice.Url = INI.JCET_WEBSERVICE_URL;
            //webservice.Url = "http://www.webxml.com.cn/WebServices/WeatherWebService.asmx";
        }
        public bool GetRecipeFromLot(string strlot,string URL)
        {
            //將來添加WEB通訊得到的數據

            string lot_id = strlot.Trim();
            string cust_device = "";
            string marking_spec = "";
            string strip_qty = "";
            string msg = "";

            try
            {
                MyLog.LogShopfloorCMD(MyLog.LogStyle.CMD_MESPlus, "MES Url =" + URL);
                //bool bOK = webservice.GetInkMarkingSpecFromPDS(lot_id, ref cust_device, ref marking_spec, ref msg);

                Hashtable pars = new Hashtable();
                String Url = URL;
                pars["LOT_ID"] = lot_id.Trim();
                pars["Cust_Device"] = "";
                pars["Marking_Spec"] = "";
                pars["Strip_Qty"] = "";
                pars["msg"] = "";
                XmlDocument doc = WebSvcCaller.QuerySoapWebService(Url, "GetInkMarkingSpecFromPDS", pars);

                //string result = webservice.GetInkMarkingSpecFromPDS(lot_id, ref cust_device, ref marking_spec, ref strip_qty, ref msg);

                XmlElement xmlElementX = doc.DocumentElement;
                XmlNodeList nodeListX = xmlElementX.ChildNodes;

                foreach (XmlNode item in nodeListX)
                {
                    if (item.Name == "Cust_Device")
                        cust_device = item.InnerText;

                    if (item.Name == "Marking_Spec")
                        marking_spec = item.InnerText;

                    if (item.Name == "Strip_Qty")
                        strip_qty = item.InnerText;

                    if (item.Name == "msg")
                        msg = item.InnerText;
                }
                string strEvo = "";



                strEvo = "Result=" + "" + Environment.NewLine;
                strEvo += "Lot_ID=" + lot_id + Environment.NewLine;
                strEvo += "Cust_Device=" + cust_device + Environment.NewLine;
                strEvo += "Marking_Spec=" + marking_spec + Environment.NewLine;
                strEvo += "Strip_Qty=" + strip_qty + Environment.NewLine;
                strEvo += "msg=" + msg + Environment.NewLine;

                m_show_message = "";
                m_show_message += "Lot_ID=" + lot_id + Environment.NewLine;
                m_show_message += "Cust_Device=" + cust_device + Environment.NewLine;
                m_show_message += "Marking_Spec=" + marking_spec + Environment.NewLine;
                m_show_message += "Strip_Qty=" + strip_qty + Environment.NewLine;


                strEvo = strEvo.Replace(Environment.NewLine, "#");
                MyLog.LogShopfloorCMD(MyLog.LogStyle.CMD_MESPlus, "MES Return Message =" + strEvo);
                if (msg == "success")
                {
                    string[] strtmp = cust_device.Split('-');

                    m_recipe_version = marking_spec + "-" + strtmp[strtmp.Length - 1];
                    m_mes_strip = int.Parse(strip_qty);
                    //m_mes_strip = 6;

                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                MyLog.LogShopfloorCMD(MyLog.LogStyle.CMD_Exception, "Message=" + ex.ToString());
                return false;
            }
        }
        public string GetVersion()
        {
            return m_recipe_version;
        }
        public int GetMESStrip()
        {
            return m_mes_strip;
        }
        public string GetShowMessage()
        {
            //Universal.ShowMessage = "";
            return m_show_message;
        }

    }

    /// <summary>
    /// 利用WebRequest/WebResponse进行WebService调用的类,By 同济黄正
    /// </summary>
    public class WebSvcCaller
    {
        //<webServices>
        //  <protocols>
        //    <add name="HttpGet"/>
        //    <add name="HttpPost"/>
        //  </protocols>
        //</webServices>

        private static Hashtable _xmlNamespaces = new Hashtable();//缓存xmlNamespace，避免重复调用GetNamespace

        /**/
        /// <summary>
        /// 需要WebService支持Post调用
        /// </summary>
        public static XmlDocument QueryPostWebService(String URL, String MethodName, Hashtable Pars)
        {
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(URL + "/" + MethodName);
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            SetWebRequest(request);
            byte[] data = EncodePars(Pars);
            WriteRequestData(request, data);

            return ReadXmlResponse(request.GetResponse());
        }
        /**/
        /// <summary>
        /// 需要WebService支持Get调用
        /// </summary>
        public static XmlDocument QueryGetWebService(String URL, String MethodName, Hashtable Pars)
        {
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(URL + "/" + MethodName + "?" + ParsToString(Pars));
            request.Method = "GET";
            request.ContentType = "application/x-www-form-urlencoded";
            SetWebRequest(request);
            return ReadXmlResponse(request.GetResponse());
        }


        /**/
        /// <summary>
        /// 通用WebService调用(Soap),参数Pars为String类型的参数名、参数值
        /// </summary>
        public static XmlDocument QuerySoapWebService(String URL, String MethodName, Hashtable Pars)
        {
            //By 同济黄正 http://hz932.ys168.com 2008-3-19
            if (_xmlNamespaces.ContainsKey(URL))
            {
                return QuerySoapWebService(URL, MethodName, Pars, _xmlNamespaces[URL].ToString());
            }
            else
            {
                return QuerySoapWebService(URL, MethodName, Pars, GetNamespace(URL));
            }
        }

        private static XmlDocument QuerySoapWebService(String URL, String MethodName, Hashtable Pars, string XmlNs)
        {
            _xmlNamespaces[URL] = XmlNs;//加入缓存，提高效率
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(URL);
            request.Method = "POST";
            request.ContentType = "text/xml; charset=utf-8";
            request.Headers.Add("SOAPAction", "" + XmlNs + (XmlNs.EndsWith("/") ? "" : "/") + MethodName + "");
            SetWebRequest(request);
            byte[] data = EncodeParsToSoap(Pars, XmlNs, MethodName);
            WriteRequestData(request, data);
            XmlDocument doc = new XmlDocument(), doc2 = new XmlDocument();
            doc = ReadXmlResponse(request.GetResponse());

            XmlNamespaceManager mgr = new XmlNamespaceManager(doc.NameTable);
            mgr.AddNamespace("soap", "http://schemas.xmlsoap.org/soap/envelope/");
            String RetXml = doc.SelectSingleNode("//soap:Body/*", mgr).InnerXml;
            doc2.LoadXml("<root>" + RetXml + "</root>");
            AddDelaration(doc2);
            return doc2;
        }
        private static string GetNamespace(String URL)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(URL + "?WSDL");
            SetWebRequest(request);
            WebResponse response = request.GetResponse();
            StreamReader sr = new StreamReader(response.GetResponseStream(), Encoding.UTF8);
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(sr.ReadToEnd());
            return doc.SelectSingleNode("//@targetNamespace").Value;
        }
        private static byte[] EncodeParsToSoap(Hashtable Pars, String XmlNs, String MethodName)
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml("<soap:Envelope xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" xmlns:soap=\"http://schemas.xmlsoap.org/soap/envelope/\"></soap:Envelope>");
            AddDelaration(doc);
            XmlElement soapBody = doc.CreateElement("soap", "Body", "http://schemas.xmlsoap.org/soap/envelope/");
            XmlElement soapMethod = doc.CreateElement(MethodName);
            soapMethod.SetAttribute("xmlns", XmlNs);
            foreach (string k in Pars.Keys)
            {
                XmlElement soapPar = doc.CreateElement(k);
                soapPar.InnerText = Pars[k].ToString();
                soapMethod.AppendChild(soapPar);
            }
            soapBody.AppendChild(soapMethod);
            doc.DocumentElement.AppendChild(soapBody);
            return Encoding.UTF8.GetBytes(doc.OuterXml);
        }

        private static void SetWebRequest(HttpWebRequest request)
        {

            request.Credentials = CredentialCache.DefaultCredentials;
            request.Timeout = 10000;
        }

        private static void WriteRequestData(HttpWebRequest request, byte[] data)
        {
            request.ContentLength = data.Length;
            Stream writer = request.GetRequestStream();
            writer.Write(data, 0, data.Length);
            writer.Close();
        }

        private static byte[] EncodePars(Hashtable Pars)
        {
            return Encoding.UTF8.GetBytes(ParsToString(Pars));
        }

        private static String ParsToString(Hashtable Pars)
        {
            StringBuilder sb = new StringBuilder();
            foreach (string k in Pars.Keys)
            {
                if (sb.Length > 0)
                {
                    sb.Append("&");
                }
                //sb.Append(HttpUtility.UrlEncode(k) + "=" + HttpUtility.UrlEncode(Pars[k].ToString()));
                sb.Append(k + "=" + Pars[k].ToString());
            }
            return sb.ToString();
        }

        private static XmlDocument ReadXmlResponse(WebResponse response)
        {
            StreamReader sr = new StreamReader(response.GetResponseStream(), Encoding.UTF8);
            String retXml = sr.ReadToEnd();
            sr.Close();
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(retXml);
            return doc;
        }

        private static void AddDelaration(XmlDocument doc)
        {
            XmlDeclaration decl = doc.CreateXmlDeclaration("1.0", "utf-8", null);
            doc.InsertBefore(decl, doc.DocumentElement);
        }
    }
}
