using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Net;
using System.Collections;
using System.IO;
using System.Xml;
using System.Globalization;
using System.Threading;
using System.Reflection;
using System.ComponentModel;

namespace APIServiceProviderNamespace
{
    public enum StatLevel
    {
        Campaign = 1,
        AdGroup = 2,
        Ad = 3,
        Keyword = 4
    }

    public class APIServiceProviderRequestError
    {
        #region Private Members

        private DateTime errorDate = DateTime.Now;
        private string methodName = "";
        private string error = "";

        #endregion

        #region Public Members

        public DateTime ErrorDate
        {
            get { return errorDate; }
            set { errorDate = value; }
        }

        public string MethodName
        {
            get { return methodName; }
            set { methodName = value; }
        }

        public string Error
        {
            get { return error; }
            set { error = value; }
        }

        #endregion

        public APIServiceProviderRequestError()
        {
        }

        public APIServiceProviderRequestError(DateTime dt, string mn, string err)
        {
            ErrorDate = dt;
            MethodName = mn;
            Error = err;
        }
    }

    public class APIServiceProviderRequest
    {
        #region Protected Members

        protected APIServiceProvider serviceProvider = null;
        private string requestLogXML = "";
        private bool isInitial = false;

        #endregion

        #region Public Members

        public APIServiceProvider ServiceProvider
        {
            get { return serviceProvider; }
            set { serviceProvider = value; }
        }

        public string RequestLogXml
        {
            get { return requestLogXML; }
            set { requestLogXML = value; }
        }

        public bool IsInitial
        {
            get { return isInitial; }
            set { isInitial = value; }
        }

        #endregion

        #region Public Methods

        public virtual string Request(params object[] parameters)
        {
            return "";
        }

        public virtual string HandleRequest(params object[] parameters)
        {
            return "";
        }

        public virtual void BeginLog(params object[] parameters)
        {
            DateTime now = DateTime.Now;
            requestLogXML = "<requestlog methodname=\"" + parameters[0].ToString() + "\" datetime=\"" + now.ToString("dd.MM.yyyy HH:mm") + "\" isinitial=\"" + isInitial.ToString() + "\">";
            requestLogXML += "<params>";
            for (int i = 1; i < parameters.Length; i++)
            {
                requestLogXML += "<param>" + parameters[i].ToString() + "</param>";
            }
            requestLogXML += "</params>";
            requestLogXML += "<logs>";
        }

        public virtual void AddLog(string msg, bool iserror)
        {
            string status = "OK";
            if (iserror)
                status = "ERROR";
            requestLogXML += "<log status=\"" + status + "\">" + msg + "</log>";
        }

        public virtual void EndLog()
        {
            requestLogXML += "</logs>";
            requestLogXML += "</requestlog>";
        }

        public virtual string ChangeDecimalSymbol(string number)
        {
            string sep = Thread.CurrentThread.CurrentCulture.NumberFormat.NumberDecimalSeparator;

            byte[] bytes = Encoding.ASCII.GetBytes(number);

            for (int i = 0; i < bytes.Length; i++)
            {
                if (bytes[i] >= '0' && bytes[i] <= '9') continue;
                if (bytes[i] != sep[0])
                    bytes[i] = (byte)sep[0];
            }

            return Encoding.ASCII.GetString(bytes);
        }

        #endregion

        #region Protected Methods

        protected string Post(string buffer)
        {
            ServicePointManager.ServerCertificateValidationCallback = new System.Net.Security.RemoteCertificateValidationCallback(IgnoreCertificateErrorHandler);

            // создаем клиента
            WebClient wc = new WebClient();

            // отправляем POST-запрос и получаем ответ
            byte[] result = wc.UploadData(ServiceProvider.Url, "POST", System.Text.Encoding.UTF8.GetBytes(buffer));

            return Encoding.UTF8.GetString(result);
        }

        protected string Get(string buffer)
        {
            ServicePointManager.ServerCertificateValidationCallback = new System.Net.Security.RemoteCertificateValidationCallback(IgnoreCertificateErrorHandler);

            WebRequest req = WebRequest.Create(ServiceProvider.Url + buffer);
            req.Method = "GET";
            WebResponse resp = req.GetResponse();

            StreamReader reader = new StreamReader(resp.GetResponseStream());
            string res = reader.ReadToEnd();
            reader.Close();
            resp.Close();

            return res;
        }

        #endregion

        #region Private Methods

        private static bool IgnoreCertificateErrorHandler(object sender,
                                                   System.Security.Cryptography.X509Certificates.X509Certificate cert,
                                                   System.Security.Cryptography.X509Certificates.X509Chain chain,
                                                   System.Net.Security.SslPolicyErrors sslErr)
        {
            return true;
        }

        #endregion

        public APIServiceProviderRequest()
        {
        }
    }

    public class YandexAPIServiceProviderRequest : APIServiceProviderRequest
    {
        #region Private Members

        int extra = 0;

        #endregion

        #region Public Members

        public override string Request(params object[] parameters)
        {
            string methodname = (string)parameters[0];
            


            string res = "";

            switch (methodname.ToLower())
            {
                case "getsummarystats":
                    res = GetSummaryStats((string)parameters[1], (string)parameters[2], (DateTime)parameters[3], (DateTime)parameters[4], (bool)parameters[5], (bool)parameters[6]);
                    break;
                case "getcampaignslist": res = GetCampaignsList((string)parameters[1], (bool)parameters[2]); break;
                case "getbanners": res = GetBanners((string)parameters[1], (bool)parameters[3], (string)parameters[2], (bool)parameters[4]); break;
                case "getreport":
                    res = GetReport((string)parameters[1], (string)parameters[2], (DateTime)parameters[3], (DateTime)parameters[4], (bool)parameters[5], (bool)parameters[6]); break;
                    //return GetReports((string)parameters[1], (string)parameters[2], new DateTime(2012, 2, 1), (DateTime)parameters[4], (bool)parameters[5]);

            }


            return res;
        }

        public override string HandleRequest(params object[] parameters)
        {
            string methodname = (string)parameters[0];

            BeginLog(parameters);

            string res = Request(parameters);

            try
            {
                switch (methodname.ToLower())
                {
                    case "getsummarystats":
                        HandleGetSummaryStats(res);
                        break;
                    case "getcampaignslist": HandleGetCampaignsList(res, "", true); break;
                    case "getsubclients": HandleGetSubClients(res); break;
                    case "getbanners": HandleGetBanners(res); break;
                    case "getreport":
                        HandleGetReport(res);
                        break;
                }
            }
            catch (Exception ex)
            {
               AddLog(methodname + " - [" + ex.Message + "]", true);
            }

            EndLog();

            return res;
        }

        #endregion

        #region Private Members

        private string CreateNewReport(string login, string campaignid, string subclient, DateTime startdate, DateTime enddate, bool isagency)
        {
            string json = "";
            string res = "";
            
          //  for (int i = 0; i < logins.Length; i++)
            {
                //if (logins[i] == null) continue;
                //string[] s = logins[i].Split(new char[1] { '=' });
                json = "{" + ServiceProvider.GetQueryString() + ", \"method\": \"CreateNewReport\", \"param\": {\"CampaignID\":\"" + campaignid +
                "\",\"StartDate\":\"" + startdate.ToString("yyyy-MM-dd") + "\",\"EndDate\":\"" + enddate.ToString("yyyy-MM-dd") + "\", \"GroupByColumns\":[\"clBanner\", \"clPhrase\", \"clDate\", \"clStatGoals\", \"clPositionType\"]} }";

                res += Post(json);
               // if (i < login.Length - 1)
                //    res += "{[(SEP)]}";
            }

            return res;
        }

        private string HandleCreateNewReport(string res)
        {
            if (res.Contains("error_str") || res.Contains("error_code"))
            {
                //AddError(DateTime.Now, "CreateNewReport", res);
                AddLog("CreateNewReport - [" + res + "]", true);
                return null;
            }

            XmlDictionaryReader xdr =
                System.Runtime.Serialization.Json.JsonReaderWriterFactory.CreateJsonReader(Encoding.UTF8.GetBytes(res),
                                                                                           XmlDictionaryReaderQuotas.Max);
            xdr.Read();
            XmlDocument xmldoc = new XmlDocument();
            string xml = xdr.ReadOuterXml();
            xmldoc.LoadXml(xml);

            xdr.Close();

            XmlElement root = xmldoc["root"];
            if (root == null) return null;

            XmlElement data = root["data"];
            if (data == null) return null;

            AddLog("CreateNewReport - [OK]", false);
            return data.InnerText;
        }

        private string DeleteReport(string reportid)
        {
            string json = "{" + ServiceProvider.GetQueryString() + ", \"method\": \"DeleteReport\", \"param\": " + reportid + " }";

            return Post(json);
        }

        private bool HandleDeleteReport(string res)
        {
            if (res.Contains("error_str") || res.Contains("error_code"))
            {
                AddLog("DeleteReport - [" + res + "]", true);
                return false;
            }

            AddLog("DeleteReport - [OK]", false);

            return true;
        }

        private void DeleteAllReports()
        {
            string[] s = HandleGetReportList(GetReportList(), "");

            for (int i = 0; i < s.Length; i++)
            {
                string[] ss = s[i].Split(new char[1] {'='});
                string res = DeleteReport(ss[0]);
            }
        }

        private string GetReportList()
        {
            string json = "{" + ServiceProvider.GetQueryString() + ", \"method\": \"GetReportList\" }";

            return Post(json);
        }

        private string[] HandleGetReportList(string res, string reportid)
        {
            if (res.Contains("error_str") || res.Contains("error_code"))
            {
                AddLog("GetReportList - [" + res + "]", true);
                return null;
            }

            XmlDictionaryReader xdr =
    System.Runtime.Serialization.Json.JsonReaderWriterFactory.CreateJsonReader(Encoding.UTF8.GetBytes(res),
                                                                               XmlDictionaryReaderQuotas.Max);


            xdr.Read();
            XmlDocument xmldoc = new XmlDocument();
            string xml = xdr.ReadOuterXml();
            xmldoc.LoadXml(xml);

            xdr.Close();

            XmlElement root = xmldoc["root"];
            if (root == null) return null;

            XmlElement data = root["data"];
            if (data == null) return null;

            ArrayList ar = new ArrayList();

            foreach (XmlNode node in data.ChildNodes)
            {
                if (node.Name == "item")
                {
                    XmlElement rid = node["ReportID"];
                    XmlElement url = node["Url"];
                    XmlElement status = node["StatusReport"];

                    if (rid == null || url == null || status == null) continue;

                    if (reportid.Length == 0 || (reportid.Length > 0 && reportid == rid.InnerText))
                    {
                        ar.Add(rid.InnerText + "=" + url.InnerText + "=" + status.InnerText);
                    }
                }
            }

            /*JObject o = JObject.Parse(res);
            int n = 0;

            JToken jt = o.SelectToken("data[0]");

            while (jt != null)
            {
                JToken rid = jt.SelectToken("ReportID");
                JToken url = jt.SelectToken("Url");
                JToken status = jt.SelectToken("StatusReport");

                if (reportid.Length == 0 || (reportid.Length > 0 && reportid == rid.ToString()))
                {
                    if (status.ToString() != "Pending" && url.ToString().Length > 0)
                    {
                        ar.Add(rid.ToString() + "=" + url.ToString());
                    }
                }

                n++;
                jt = o.SelectToken("data[" + n.ToString() + "]");
            }*/

            string[] urls = new string[ar.Count];
            for (int i = 0; i < ar.Count; i++)
            {
                urls[i] = ar[i].ToString();
            }

            AddLog("GetReportList - [OK]", false);

            return urls;
        }

        private string GetReport(string login, string subclient, DateTime startdate, DateTime enddate, bool isagency, bool activecampaignsonly)
        {
            string[] logins = HandleGetCampaignsList(GetCampaignsList(login, isagency), subclient, activecampaignsonly);

            if (logins == null)
            {
                return "{\"error_str\": \"Campaigns id list is null\"}";
            }

            DeleteAllReports();

            string res = "";
            
            for (int i = 0; i < logins.Length; i++)
            {
                if (logins[i] == null) continue;

                string[] s = logins[i].Split(new char[1] { '=' });
                string rid = HandleCreateNewReport(CreateNewReport(login, s[0], subclient, startdate, enddate, isagency));

                if (rid == null)
                {
                    continue;
                }

                System.Threading.Thread.Sleep(20000);
                string[] rep = HandleGetReportList(GetReportList(), rid);
                while (rep.Length == 0)
                {
                    System.Threading.Thread.Sleep(20000);
                    rep = HandleGetReportList(GetReportList(), rid);
                }

                if (rep == null) continue;

                s = rep[0].Split(new char[1] { '=' });

                if (s.Length == 3 && rep.Length > 0 && s[0] == rid && s[1].Length > 0 && s[2] != "Pending")
                {
                    WebClient client = new WebClient();
                    byte[] bytes = client.DownloadData(s[1]);

                    res += StringCompressor.CompressString(Encoding.UTF8.GetString(bytes));

                    //if (i < login.Length - 1)
                        res += "{[(SEP)]}";
                }

                string ss = DeleteReport(rid);
            }

            return res;
        }

        private void HandleGetReport(string res)
        {
            string[] strs = res.Split(new string[1] {"{[(SEP)]}"}, StringSplitOptions.None);

            XmlDocument xmldoc = new XmlDocument();

            for (int i = 0; i < strs.Length; i++)
            {
                string str = "";
                
                if (strs[i].Length > 0) 
                    str = StringCompressor.DecompressString(strs[i]);

                if (str.Contains("error_str") || str.Contains("error_code") || str.Length == 0)
                {
                    //AddError(DateTime.Now, "GetReport", res);
                    if (str.Length > 0)
                        AddLog("GetReport - [" + str + "]", true);
                    continue;
                }

                try
                {
                    xmldoc.LoadXml(str);
                }
                catch (Exception exobj)
                {
                    continue;
                }

                XmlElement rootel = xmldoc["report"];

                if (rootel == null) continue;

                XmlElement campaignIDEl = rootel["campaignID"];

                if (campaignIDEl == null) continue;

                XmlElement phrasesDicEl = rootel["phrasesDict"];

                if (phrasesDicEl == null) continue;

                Hashtable phrasesDic = new Hashtable();

                foreach (XmlNode node in phrasesDicEl.ChildNodes)
                {
                    if (node.Name == "phrase")
                    {
                        XmlAttribute phraseIDAtt = node.Attributes["phraseID"];
                        XmlAttribute valueAtt = node.Attributes["value"];

                        if (phraseIDAtt != null && valueAtt != null)
                        {
                            phrasesDic[phraseIDAtt.Value] = valueAtt.Value;
                        }
                    }
                }

                XmlElement statEl = rootel["stat"];

                if (statEl == null) continue;

                foreach (XmlNode node in statEl.ChildNodes)
                {
                    if (node.Name == "row")
                    {
                        string df = "yyyy-MM-dd";
                        if (ServiceProvider.Formats["datetime"] != null)
                            df = ServiceProvider.Formats["datetime"].ToString();

                        XmlAttribute statDateAtt = node.Attributes["statDate"];
                        XmlAttribute sumAtt = node.Attributes["sum"];
                        XmlAttribute showsAtt = node.Attributes["shows"];
                        XmlAttribute clicksAtt = node.Attributes["clicks"];
                        XmlAttribute phraseIDAtt = node.Attributes["phraseID"];

                        if (statDateAtt == null || sumAtt == null || showsAtt == null || clicksAtt == null || phraseIDAtt == null)
                            continue;

                        string phrase = phrasesDic[phraseIDAtt.Value].ToString();

                        string dataxml = "<row />";

                        //foreach(XmlAttribute att in node.Attributes)
                        //{
                        //    dataxml += att.Name + "=\"" + att.Value + "\" ";
                        //}

                        //dataxml += " phrase=\"" + phrase + "\" />";

                        

                        APIServiceProviderNamespace.main.campaignsDataTable dt = DBModule.GetCampaignByCID(campaignIDEl.InnerText);

                        if (dt != null && dt.Rows.Count > 0)
                        {
                            if (!DBModule.AddStatisticsRecord(Convert.ToInt32(dt.Rows[0]["id"]), DateTime.ParseExact(statDateAtt.Value, df, null), Convert.ToInt32(showsAtt.Value), Convert.ToInt32(clicksAtt.Value), Convert.ToDecimal(ChangeDecimalSymbol(sumAtt.Value)), dataxml, StatLevel.Keyword))
                                AddLog("GetReport(AddStatisticsRecord) - [" + DBModule.lastErrorString + "]", true);
                        }
                        else
                        {
                            if (dt == null)
                                AddLog("GetReport(GetCampaignByCID) - [" + DBModule.lastErrorString + "]", true);
                            else
                                AddLog("GetReport(GetCampaignByCID) - [Campaign by cid is not found]", true);
                        }
                    }
                }
            }

            AddLog("GetReport - [OK]", false);

        }

        private string GetSummaryStats(string login, string subclient, DateTime startdate, DateTime enddate, bool isagency, bool activecampaignsonly)
        {
            string[] logins = HandleGetCampaignsList(GetCampaignsList(login, isagency), subclient, activecampaignsonly);

            if (logins == null)
            {
                return "{\"error_str\": \"Campaigns id list is null\"}";
            }

            string res = "";

            string json = "";

            DateTime sd = startdate;
            DateTime ed = enddate;

            TimeSpan ts = enddate - startdate;

            if (ts.Days <= 30)
            {
                json = "{" + ServiceProvider.GetQueryString() + ", \"method\": \"GetSummaryStat\", \"param\": {\"CampaignIDS\":[";

                for (int i = 0; i < logins.Length; i++)
                {
                    if (logins[i] == null) continue;
                    string[] s = logins[i].Split(new char[1] { '=' });
                    json += "\"" + s[0] + "\"";
                    if (i < logins.Length - 1)
                        json += ",";
                }

                if (json[json.Length - 1] == ',')
                    json = json.Remove(json.Length - 1);

                json += "],\"StartDate\":\"" + sd.ToString("yyyy-MM-dd") + "\",\"EndDate\":\"" + ed.ToString("yyyy-MM-dd") + "\"} }";

                return Post(json);
            }

            int n = ts.Days;

            ed = sd.AddDays(30);

            bool isbreak = false;

            while (ed <= enddate)
            {
                json = "{" + ServiceProvider.GetQueryString() + ", \"method\": \"GetSummaryStat\", \"param\": {\"CampaignIDS\":[";

                for (int i = 0; i < logins.Length; i++)
                {
                    if (logins[i] == null) continue;
                    string[] s = logins[i].Split(new char[1] { '=' });
                    json += "\"" + s[0] + "\"";
                    if (i < logins.Length - 1)
                        json += ",";
                }

                if (json[json.Length - 1] == ',')
                    json = json.Remove(json.Length - 1);

                json += "],\"StartDate\":\"" + sd.ToString("yyyy-MM-dd") + "\",\"EndDate\":\"" + ed.ToString("yyyy-MM-dd") + "\"} }";

                sd = sd.AddDays(30);
                ed = sd.AddDays(30);

                if (res.Length > 0)
                    res += "{[(SEP)]}";

                res += Post(json);

                if (isbreak) break;

                if (ed > enddate)
                {
                    ed = enddate;
                    isbreak = true;
                }
            }

            return res;
        }

        private object HandleGetSummaryStats(string res)
        {
            if (res.Contains("error_str") || res.Contains("error_code"))
            {
               // AddError(DateTime.Now, "GetSummaryStats", res);
                AddLog("GetSummaryStats - [" + res + "]", true);
                return null;
            }

            string[] results = res.Split(new string[1] { "{[(SEP)]}" }, StringSplitOptions.None);

            for (int j = 0; j < results.Length; j++)
            {
                if (results[j] == "{\"data\":[]}") continue;


                XmlDictionaryReader xdr =
    System.Runtime.Serialization.Json.JsonReaderWriterFactory.CreateJsonReader(Encoding.UTF8.GetBytes(results[j]),
                                                                               XmlDictionaryReaderQuotas.Max);
                xdr.Close();
               /* JsonTextParser parser = new JsonTextParser();
                JsonObject obj = parser.Parse(results[j]);
                JsonUtility.GenerateIndentedJsonText = false;
                JsonObjectCollection col = (JsonObjectCollection)obj;

                JsonArrayCollection ar = col[0] as JsonArrayCollection;

                for (int i = 0; i < ar.Count; i++)
                {
                    JsonObjectCollection oc = ar[i] as JsonObjectCollection;
                    APIServiceProviderNamespace.main.campaignsDataTable dt = DBModule.GetCampaignByCID(oc["CampaignID"].GetValue().ToString());
                    if (dt.Rows.Count > 0)
                    {
                        string data = "<fields>";
                        data += "<sumsearch>" + oc["SumSearch"].GetValue().ToString() + "</sumsearch>";
                        data += "<sumcontext>" + oc["SumContext"].GetValue().ToString() + "</sumcontext>";
                        data += "<showssearch>" + oc["ShowsSearch"].GetValue().ToString() + "</showssearch>";
                        data += "<showscontext>" + oc["ShowsContext"].GetValue().ToString() + "</showscontext>";
                        data += "<clickssearch>" + oc["ClicksSearch"].GetValue().ToString() + "</clickssearch>";
                        data += "<clickscontext>" + oc["ClicksContext"].GetValue().ToString() + "</clickscontext>";
                        data += "</fields>";

                        string df = "yyyy-MM-dd"; 
                        if (ServiceProvider.Formats["datetime"] != null)
                            df = ServiceProvider.Formats["datetime"].ToString();

                        DateTime statdate = DateTime.ParseExact(oc["StatDate"].GetValue().ToString(), df, null);
                       // DBModule.AddStatisticsRecord(Convert.ToInt32(dt.Rows[0]["id"]), statdate, Convert.ToInt32(oc["ShowsSearch"].GetValue()), Convert.ToInt32(oc["ClicksSearch"].GetValue()), data);
                    }
                }*/
            }

            AddLog("GetSummaryStats - [OK]", false);
            return new object();
        }

        private string GetCampaignsList(string login, bool isagency)
        {
            string[] logins = HandleGetSubClients(GetSubClients(login));

            if (logins == null && isagency)
            {
                return "{\"error_str\": \"sub-clients list is null\"}";
            }

            string json = "{" + ServiceProvider.GetQueryString() + ", \"method\": \"GetCampaignsList\"";
            if (isagency)
            {
                json += ", \"param\": [";

                int len = logins.Length;

                for (int i = 0; i < len; i++)
                {
                    json += "\"" + logins[i] + "\"";
                    if (i < len - 1)
                        json += ",";
                }

                json += " ]";
            }

            json += " }";

            return Post(json);
        }

        private string[] HandleGetCampaignsList(string res, string subclient, bool activesonly)
        {
            if (res.Contains("error_str") || res.Contains("error_code"))
            {
                //AddError(DateTime.Now, "GetCampaignList", res);
                AddLog("GetCampaignList - [" + res + "]", true);
                return null;
            }

            if (res == "{\"data\":[]}") return null;

            XmlDictionaryReader xdr =
System.Runtime.Serialization.Json.JsonReaderWriterFactory.CreateJsonReader(Encoding.UTF8.GetBytes(res),
                                                                           XmlDictionaryReaderQuotas.Max);

            xdr.Read();
            XmlDocument xmldoc = new XmlDocument();
            string xml = xdr.ReadOuterXml();
            xmldoc.LoadXml(xml);

            xdr.Close();

            XmlElement root = xmldoc["root"];
            if (root == null) return null;

            XmlElement data = root["data"];
            if (data == null) return null;

            ArrayList ar = new ArrayList();

            foreach (XmlNode node in data.ChildNodes)
            {
                if (node.Name == "item")
                {
                    XmlElement isactive = node["IsActive"];
                    XmlElement name = node["Name"];
                    XmlElement id = node["CampaignID"];
                    XmlElement login = node["Login"];

                    if (isactive == null || name == null || id == null || login == null) continue;

                    if (subclient.Length > 0 && subclient != login.InnerText) continue;

                    if (!activesonly || (activesonly && isactive.InnerText.ToLower() == "yes"))
                    {
                        string s = id.InnerText;
                        int cid = DBModule.AddCompaign(name.InnerText, id.InnerText, SyncManager.CurExecProject.DbId, ServiceProvider.Name, login.InnerText);
                        if (cid == 0)
                        {
                            AddLog("GetCampaignList(AddCampaign) - [" + DBModule.lastErrorString + "]", true);
                        }
                        else
                        {
                            s += "=" + cid.ToString();
                            ar.Add(s);
                        }
                    }
                }
            }



            string[] compaignids = new string[ar.Count];

            for (int i = 0; i < ar.Count; i++)
                compaignids[i] = ar[i].ToString();

            AddLog("GetCampaignList - [OK]", false);
            return compaignids;
        }


        private string GetSubClients(string login)
        {
            string json = "{" + ServiceProvider.GetQueryString() + ", \"method\": \"GetSubClients\", \"param\": {\"Login\":\"" + login + "\"} }";
            return Post(json);
        }

        private string[] HandleGetSubClients(string res)
        {
            if (res.Contains("error_str") || res.Contains("error_code"))
            {
                //AddError(DateTime.Now, "GetSubClients", res);
                AddLog("GetSubClients - [" + res + "]", true);
                return null;
            }

            if (res == "{\"data\":[]}") return null;

            XmlDictionaryReader xdr =
System.Runtime.Serialization.Json.JsonReaderWriterFactory.CreateJsonReader(Encoding.UTF8.GetBytes(res),
                                                               XmlDictionaryReaderQuotas.Max);

            xdr.Read();
            XmlDocument xmldoc = new XmlDocument();
            string xml = xdr.ReadOuterXml();
            xmldoc.LoadXml(xml);

            xdr.Close();

            XmlElement root = xmldoc["root"];
            if (root == null) return null;

            XmlElement data = root["data"];
            if (data == null) return null;

            ArrayList ar = new ArrayList();

            foreach (XmlNode node in data.ChildNodes)
            {
                if (node.Name == "item" && node["Login"] != null)
                {
                    ar.Add(node["Login"].InnerText);
                }
            }

            string[] logins = new string[ar.Count];

            for (int i = 0; i < ar.Count; i++)
                logins[i] = ar[i].ToString();

            AddLog("GetSubClients - [OK]", false);
            return logins;
        }


        private string GetBanners(string login, bool isagency, string subclient, bool activecampaignsonly)
        {
            string[] logins = HandleGetCampaignsList(GetCampaignsList(login, isagency), subclient, activecampaignsonly);

            if (logins == null)
            {
                return "{\"error_str\": \"Campaigns id list is null\"}";
            }

            string json = "{" + ServiceProvider.GetQueryString() + ", \"method\": \"GetBanners\", \"param\": {\"CampaignIDS\":[";

            int n = 0;
            int i = 0;

            string res = "";
 
            for (i = 0; i < logins.Length; i++)
            {
                if (logins[i] == null) continue;
                n++;
                string[] s = logins[i].Split(new char[1] { '=' });
                json = "{" + ServiceProvider.GetQueryString() + ", \"method\": \"GetBanners\", \"param\": {\"CampaignIDS\":[";
                json += "\"" + s[0] + "\"";
//               if (i < logins.Length - 1)
  //                  json += ",";
                json += "],\"GetPhrases\":\"Yes\",\"Filter\":{ \"IsActive\": [\"Yes\"] }} }";
                res += Post(json);
                if (i < logins.Length - 1)
                    res += "{[(SEP)]}";
            }

            return res;
        }

        private object HandleGetBanners(string res)
        {
            if (res.Contains("error_str") || res.Contains("error_code"))
            {
                //AddError(DateTime.Now, "GetSubClients", res);
                AddLog("GetBanners - [" + res + "]", true);
                return null;
            }

            string[] results = res.Split(new string[1] { "{[(SEP)]}" }, StringSplitOptions.RemoveEmptyEntries);

            for (int i = 0; i < results.Length; i++)
            {
                if (results[i] == "{\"data\":[]}") continue;

                //XmlDocument doc = (XmlDocument)JsonConvert.DeserializeXmlNode(results[i]);

                XmlDictionaryReader xdr =
                    System.Runtime.Serialization.Json.JsonReaderWriterFactory.CreateJsonReader(
                        Encoding.UTF8.GetBytes(results[i]),
                        XmlDictionaryReaderQuotas.Max);

                xdr.Read();
                XmlDocument xmldoc = new XmlDocument();
                string xml = xdr.ReadOuterXml();
                xmldoc.LoadXml(xml);

                xdr.Close();

                XmlElement root = xmldoc["root"];
                if (root == null) return null;

                XmlElement data = root["data"];
                if (data == null) return null;

                foreach (XmlNode node in data.ChildNodes)
                {
                    if (node.Name == "item")
                    {
                        XmlNode phrases = node["Phrases"];
                        if (phrases == null) continue;
                        foreach (XmlNode node1 in phrases.ChildNodes)
                        {
                            if (node1.Name == "item")
                            {
                                XmlElement campaignid = node1["CampaignID"];
                                XmlElement phrase = node1["Phrase"];
                                XmlElement phraseid = node1["PhraseID"];
                                if (campaignid == null || phrase == null || phraseid == null) continue;
                                APIServiceProviderNamespace.main.campaignsDataTable dt =
                                    DBModule.GetCampaignByCID(campaignid.InnerText);
                                if (dt != null && dt.Rows.Count > 0)
                                {
                                    if (!DBModule.AddKeyword(Convert.ToInt32(dt.Rows[0]["id"]), phraseid.InnerText,
                                                        phrase.InnerText))
                                    {
                                        AddLog("GetBanners(AddKeyword) - [" + DBModule.lastErrorString + "]", true);
                                    }
                                }
                                else
                                {
                                    if (dt == null)
                                        AddLog("GetBanners(GetCampaignByCID) - [" + DBModule.lastErrorString + "]", true);
                                    else
                                        AddLog("GetBanners(GetCampaignByCID) - [Campaign by cid not found]", true);
                                }
                            }
                        }
                    }
                }
            }


            /* decimal dprice = 0;
                           if (price != null && !decimal.TryParse(ChangeDecimalSymbol(price.ToString()), out dprice))
                               dprice = 0;

                           decimal dcontextprice = 0;
                           if (contextprice != null && !decimal.TryParse(ChangeDecimalSymbol(contextprice.ToString()), out dcontextprice))
                               dcontextprice = 0;

                           decimal dcurrentonsearch = 0;
                           if (currentonsearch != null && !decimal.TryParse(ChangeDecimalSymbol(currentonsearch.ToString()), out dcurrentonsearch))
                               dcurrentonsearch = 0;


                           bool blowctrwarning = false;
                           if (lowctrwarning != null && lowctrwarning.ToString() == "Yes")
                               blowctrwarning = true;

                           bool blowctr = false;
                           if (blowctr != null && lowctr.ToString() == "Yes")
                               blowctr = true;

                           bool bcontextlowctr = false;
                           if (contextlowctr != null && contextlowctr.ToString() == "Yes")
                               bcontextlowctr = true;*/

            AddLog("GetBanners - [OK]", false);
            return new object();
        }

        #endregion


        public YandexAPIServiceProviderRequest()
        {
        }
    }

    public class OrdersAPIServiceProviderRequest : APIServiceProviderRequest
    {
        #region Public Members

        public override string Request(params object[] parameters)
        {
            string methodname = (string)parameters[0];

            switch (methodname.ToLower())
            {
                case "getorders": return GetOrders((DateTime)parameters[1], (DateTime)parameters[2]);
                case "getorderdetails": return GetOrderDetails((int)parameters[1]);
            }

            return "";
        }

        public override string HandleRequest(params object[] parameters)
        {
            string methodname = (string)parameters[0];

            string res = Request(parameters);

            try
            {
                switch (methodname.ToLower())
                {
                    case "getorders": HandleGetOrders(res); break;
                    case "getorderdetails": HandleGetOrderDetails(res); break;
                } 
            }
            catch(Exception ex)
            {
                AddLog(methodname + " - [" + ex.Message + "]", true);
            }

            return res;
        }

        #endregion

        #region Private Members

        private string GetOrders(DateTime startdate, DateTime enddate)
        {
            DateTime dt = startdate;

            string orders_page = ServiceProvider.RequiredParams["orders_page"].ToString();

            string xml = "";

            while (dt <= enddate)
            {
                if (dt != startdate)
                    xml += "[{(XML_SEP)}]";
                xml += StringCompressor.CompressString(Get(orders_page + "?date=" + dt.ToString("dd.MM.yyyy")));

                dt = dt.AddDays(1);
            }

            return xml;
        }

        private object HandleGetOrders(string res)
        {
            if (res.Length == 0)
            {
                //AddError(DateTime.Now, "GetOrders", res);
                AddLog("GetOrders - [NO DATA]", true);
                return null;
            }

            XmlDocument xmldoc = new XmlDocument();

            string[] xmls = res.Split(new string[1] { "[{(XML_SEP)}]" }, StringSplitOptions.None);

            for (int m = 0; m < xmls.Length; m++)
            {
                string xmlstr = StringCompressor.DecompressString(xmls[m]);
                xmldoc.LoadXml(xmlstr);

                XmlElement root = xmldoc["orders"];

                if (root == null) return null;

                foreach (XmlElement node in root.ChildNodes)
                {
                    if (node.Name.ToLower() == "order")
                    {
                        XmlAttribute id = node.Attributes["id"];
                        if (id == null) continue;

                        XmlNode status = node["status"];
                        XmlNode datetodeliver = node["datetodeliver"];
                        XmlNode deliverycost = node["deliverycost"];
                        XmlNode deliveryprice = node["deliveryprice"];
                        XmlNode iscash = node["iscash"];
                        XmlNode additionaldeliverysum = node["additionaldeliverysum"];
                        XmlNode sum = node["sum"];
                        XmlNode discounted = node["discounted"];

                        if (status == null || datetodeliver == null || deliverycost == null || deliveryprice == null || iscash == null || additionaldeliverysum == null || sum == null || discounted == null)
                            continue;

                        DateTime dt = DateTime.Now;
                        if (!DateTime.TryParse(datetodeliver.InnerText, out dt))
                            dt = DateTime.Now;

                        string details = GetOrderDetails(Convert.ToInt32(id.Value));

                        XmlDocument dxml = new XmlDocument();
                        dxml.LoadXml(details);

                        XmlElement droot = dxml["order"];

                        XmlNode orderdate = droot["DateOrdered"];

                        if (orderdate == null) continue;

                        string dtformat = "dd.MM.yyyy";
                        if (ServiceProvider.Formats["datetime"] != null && ServiceProvider.Formats["datetime"].ToString().Length > 0)
                            dtformat = ServiceProvider.Formats["datetime"].ToString();


                        DateTime datetime = DateTime.ParseExact(orderdate.InnerText, dtformat, null);
                        //datetime = DateTime.SpecifyKind(datetime, DateTimeKind.Local);

                        if (dt.Year < 1973)
                            dt = DateTime.ParseExact(orderdate.InnerText, dtformat, null).AddDays(1);

                        decimal dc = 0;
                        try
                        {
                            dc = Convert.ToDecimal(ChangeDecimalSymbol(deliverycost.InnerText));
                        }
                        catch
                        {
                        }
                        decimal dp = 0;
                        try
                        {
                            dp = Convert.ToDecimal(ChangeDecimalSymbol(deliveryprice.InnerText));
                        }
                        catch
                        {
                        }
                        decimal ads = 0;
                        try
                        {
                            ads = Convert.ToDecimal(ChangeDecimalSymbol(additionaldeliverysum.InnerText));
                        }
                        catch
                        {
                        }
                        decimal sm = 0;
                        try
                        {
                            sm = Convert.ToDecimal(ChangeDecimalSymbol(sum.InnerText));
                        }
                        catch
                        {
                        }
                        decimal ds = 0;
                        try
                        {
                            ds = Convert.ToDecimal(ChangeDecimalSymbol(discounted.InnerText));
                        }
                        catch
                        {
                        }

                        int oid = DBModule.AddOrder(Convert.ToInt32(id.Value), status.InnerText, datetime, dt, dc, dp, Convert.ToBoolean(iscash.InnerText), ads, sm, ds, ServiceProvider.Name, SyncManager.CurExecProject.DbId);

                        if (oid == 0)
                        {
                            AddLog("GetOrders(AddOrder) - [" + DBModule.lastErrorString + "]", true);
                            continue;
                        }

                        XmlElement products = droot["orderedProducts"];

                        if (products == null) continue;

                        foreach (XmlNode node1 in products.ChildNodes)
                        {
                            if (node1.Name.ToLower() != "product") continue;

                            XmlAttribute pid = node1.Attributes["id"];
                            XmlNode ProductTitle = node1["ProductTitle"];
                            XmlNode ProductStatus = node1["ProductStatus"];
                            XmlNode Quantity = node1["Quantity"];
                            XmlNode SingleProductPrice = node1["SingleProductPrice"];
                            XmlNode TotalSum = node1["TotalSum"];
                            XmlNode DiscountedPrice = node1["DiscountedPrice"];

                            if (pid == null || ProductTitle == null || ProductStatus == null || Quantity == null || SingleProductPrice == null || TotalSum == null || DiscountedPrice == null) continue;

                            double qty = 0;
                            try
                            {
                                qty = Convert.ToDouble(ChangeDecimalSymbol(Quantity.InnerText));
                            }
                            catch
                            {
                            }
                            decimal ts = 0;
                            try
                            {
                                ts = Convert.ToDecimal(ChangeDecimalSymbol(TotalSum.InnerText));
                            }
                            catch
                            {
                            }
                            dp = 0;
                            try
                            {
                                dp = Convert.ToDecimal(ChangeDecimalSymbol(DiscountedPrice.InnerText));
                            }
                            catch
                            { 
                            }
                            decimal spp = 0;
                            try
                            {
                                spp = Convert.ToDecimal(ChangeDecimalSymbol(SingleProductPrice.InnerText));
                            }
                            catch
                            {
                            }


                            if (!DBModule.AddOrderDetails(oid, Convert.ToInt32(pid.Value), ProductTitle.InnerText, ProductStatus.InnerText, (float)qty, spp, ts, dp, false))
                            {
                                AddLog("GetOrders(AddOrderDetails) - [" + DBModule.lastErrorString + "]", true);
                            }
                        }

                        products = droot["returnedProducts"];

                        if (products == null) continue;

                        foreach (XmlNode node1 in products.ChildNodes)
                        {
                            if (node1.Name.ToLower() != "product") continue;

                            XmlAttribute pid = node1.Attributes["id"];
                            XmlNode ProductTitle = node1["ProductTitle"];
                            XmlNode Quantity = node1["Quantity"];
                            XmlNode TotalSum = node1["TotalSum"];

                            if (pid == null || ProductTitle == null || Quantity == null || TotalSum == null) continue;


                            double qty = Convert.ToDouble(ChangeDecimalSymbol(Quantity.InnerText));
                            decimal ts = Convert.ToDecimal(ChangeDecimalSymbol(TotalSum.InnerText));

                            if (!DBModule.AddOrderDetails(oid, Convert.ToInt32(pid.Value), ProductTitle.InnerText, "", (float)qty, 0, ts, 0, true))
                            {
                                AddLog("GetOrders(AddOrderDetails) - [" + DBModule.lastErrorString + "]", true);
                            }
                        }

                    }
                }

            }

            AddLog("GetOrders - [OK]", false);
            return null;
        }

        private string GetOrderDetails(int orderid)
        {
            string details_page = ServiceProvider.RequiredParams["details_page"].ToString();
            return Get(details_page + "?order_id=" + orderid.ToString());
        }

        private object HandleGetOrderDetails(string res)
        {
            AddLog("GetOrderDetails - [OK]", false);
            return new object();
        }

        #endregion


        public OrdersAPIServiceProviderRequest()
        {
        }
    }
}
