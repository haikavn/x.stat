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

        private bool hasError = false;
        private string executingMethodName = "";
        private string executingErrorMsg = "";

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

        public bool HasError
        {
            get { return hasError; }
            set { hasError = value; }
        }

        public string ExecutingMethodName
        {
            get { return executingMethodName; }
            set { executingMethodName = value; }
        }

        public string ExecutingErrorMsg
        {
            get { return executingErrorMsg; }
            set { executingErrorMsg = value; }
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
            hasError = false;
            DateTime now = SyncManager.ExecutingDate;
            executingMethodName = parameters[0].ToString();

            requestLogXML = "<requestlog methodname=\"" + parameters[0].ToString() + "\" datetime=\"" + now.ToString("dd.MM.yyyy HH:mm:ss") + "\" isinitial=\"" + isInitial.ToString() + "\">";
            requestLogXML += "<params>";
            for (int i = 1; i < parameters.Length; i++)
            {
                if (parameters[i].GetType() != typeof(DateTime))
                    requestLogXML += "<param type=\"" + parameters[i].GetType().ToString() + "\">" + parameters[i].ToString() + "</param>";
                else
                    requestLogXML += "<param type=\"" + parameters[i].GetType().ToString() + "\">" + ((DateTime)parameters[i]).ToString("dd.MM.yyyy") + "</param>";
            }
            requestLogXML += "</params>";
            requestLogXML += "<logs>";
        }

        public virtual void AddLog(string msg, bool iserror)
        {
            string status = "OK";
            if (iserror)
            {
                status = "ERROR";
            }

            executingErrorMsg = msg;

            if (!hasError)
                hasError = iserror;

            requestLogXML += "<log status=\"" + status + "\">" + msg + "</log>";
        }

        public virtual void EndLog()
        {
            requestLogXML += "</logs>";
            if (hasError)
                requestLogXML += "<result>error</result>";
            else
                requestLogXML += "<result>success</result>";

            requestLogXML += "<provider>" + ServiceProvider.GetType().ToString() + "</provider>";
            requestLogXML += "<providername>" + ServiceProvider.Name + "</providername>";
            requestLogXML += "<project>" + SyncManager.CurExecProject.DbId.ToString() + "</project>";
            requestLogXML += "</requestlog>";

            DBModule.AddExecLog(SyncManager.CurExecProject.DbId, SyncManager.ExecutingDate, hasError, executingErrorMsg, executingMethodName);
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

            string res = "";

            try
            {
                // создаем клиента
                WebClient wc = new WebClient();

                // отправляем POST-запрос и получаем ответ
                byte[] result = wc.UploadData(ServiceProvider.Url, "POST", System.Text.Encoding.UTF8.GetBytes(buffer));

                res = Encoding.UTF8.GetString(result);
            }
            catch
            {
                res = "{\"error_str\": \"Post failed\"}";
            }

            return res;
        }

        protected string Get(string buffer)
        {
            ServicePointManager.ServerCertificateValidationCallback = new System.Net.Security.RemoteCertificateValidationCallback(IgnoreCertificateErrorHandler);

            string res = "";

            try
            {
                WebRequest req = WebRequest.Create(ServiceProvider.Url + buffer);
                req.Method = "GET";
                WebResponse resp = req.GetResponse();

                StreamReader reader = new StreamReader(resp.GetResponseStream());
                res = reader.ReadToEnd();
                reader.Close();
                resp.Close();
            }
            catch (Exception ex)
            {
                return "{\"error_str\": \"Get failed\"}";
            }

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
                if (!res.Contains("\"error_code\":2"))
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
                return StringCompressor.CompressString("{\"error_str\": \"Campaigns id list is null\"}");
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

                System.Threading.Thread.Sleep(10000);
                string[] rep = HandleGetReportList(GetReportList(), rid);

                int rc = 0;

                while (rep.Length == 0 && rep != null)
                {
                    if (rc > 50) break;
                    System.Threading.Thread.Sleep(10000);
                    rep = HandleGetReportList(GetReportList(), rid);
                    rc++;
                }

                if (rep == null)
                {
                    DeleteReport(rid);
                    continue;
                }

                s = rep[0].Split(new char[1] { '=' });

                if (s.Length == 3 && rep.Length > 0 && s[0] == rid && s[1].Length > 0 && s[2] != "Pending")
                {
                    WebClient client = new WebClient();
                    byte[] bytes = null;

                    try
                    {
                        bytes = client.DownloadData(s[1]);
                        res += StringCompressor.CompressString(Encoding.UTF8.GetString(bytes));
                    }
                    catch(Exception ex)
                    {
                        bytes = null;
                        DeleteReport(rid);
                        res += StringCompressor.CompressString("{\"error_str\": \"" + ex.Message + "\"}");
                    }

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

            Hashtable dates = new Hashtable();

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
                    {
                        AddLog("GetReport - [" + str + "]", true);
                        return;
                    }
                }

                if (str.Length > 0)
                {
                    try
                    {
                        xmldoc.LoadXml(str);
                    }
                    catch (Exception exobj)
                    {
                        AddLog("GetReport - [Error loading xml][" + exobj.Message + "]", true);
                        return;
                    }
                }
            }

            for (int i = 0; i < strs.Length; i++)
            {
                string str = "";
                
                if (strs[i].Length > 0) 
                    str = StringCompressor.DecompressString(strs[i]);

                if (str.Length == 0) continue;

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
                            if (dates[statDateAtt.Value] == null)
                            {
                                DBModule.DeleteStatisticsRecords(SyncManager.CurExecProject.DbId, DateTime.ParseExact(statDateAtt.Value, df, null));
                                dates[statDateAtt.Value] = true;
                            }
                            if (!DBModule.AddStatisticsRecord(Convert.ToInt32(dt.Rows[0]["id"]), DateTime.ParseExact(statDateAtt.Value, df, null), Convert.ToInt32(showsAtt.Value), Convert.ToInt32(clicksAtt.Value), Convert.ToDecimal(ChangeDecimalSymbol(sumAtt.Value)), dataxml, StatLevel.Keyword, SyncManager.CurExecProject.DbId, phrase))
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
                //AddLog("GetSummaryStats - [" + res + "]", true);
                return null;
            }

            decimal sum = 0;

            string[] results = res.Split(new string[1] { "{[(SEP)]}" }, StringSplitOptions.None);

            for (int j = 0; j < results.Length; j++)
            {
                if (results[j] == "{\"data\":[]}") continue;


                XmlDictionaryReader xdr =
    System.Runtime.Serialization.Json.JsonReaderWriterFactory.CreateJsonReader(Encoding.UTF8.GetBytes(results[j]),
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
                        try
                        {
                            sum += decimal.Parse(node["SumSearch"].InnerText);
                        }
                        catch
                        {
                        }
                    }
                }

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
            return sum;
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
                        Hashtable ht = HandleGetCampaignParams(GetCampaignParams(id.InnerText));

                        decimal balance = 0;
                        decimal maxtransfer = 0;

                        try
                        {
                            balance = Convert.ToDecimal(ht["Rest"]);
                            maxtransfer = Convert.ToDecimal(ht["SumAvailableForTransfer"]);
                        }
                        catch
                        {
                        }

                        int cid = DBModule.AddCompaign(name.InnerText, id.InnerText, SyncManager.CurExecProject.DbId, ServiceProvider.Name, login.InnerText, APIServiceProviderTypes.YandexDirect, balance, maxtransfer);
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

        public string GetCampaignParams(string campaignid)
        {
            string json = "{" + ServiceProvider.GetQueryString() + ", \"method\": \"GetCampaignParams\", \"param\": {\"CampaignID\":\"" + campaignid + "\"} }";
            return Post(json);
        }

        public Hashtable HandleGetCampaignParams(string res)
        {
            if (res.Contains("error_str") || res.Contains("error_code"))
            {
                //AddError(DateTime.Now, "GetSubClients", res);
                AddLog("HandleGetCampaignParams - [" + res + "]", true);
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

            Hashtable ht = new Hashtable();

            foreach (XmlNode node in data.ChildNodes)
            {
                XmlAttribute type = node.Attributes["type"];

                if (type == null) continue;

                if (type.Value != "object")
                {
                    ht[node.Name] = node.InnerText;
                }
            }

            return ht;
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

            BeginLog(parameters);

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

            EndLog();

            return res;
        }

        #endregion

        #region Private Members

        private XmlNode ConvertXmlElementToLowerCase(XmlDocument xmldoc, XmlNode node)
        {
            XmlNode nd = xmldoc.CreateNode(node.NodeType, node.Name.ToLower(), "");
            if (node.Value != null)
                nd.Value = node.Value;

            if (node.Attributes != null)
            foreach (XmlAttribute att in node.Attributes)
            {
                XmlAttribute at = xmldoc.CreateAttribute(att.Name.ToLower());
                at.Value = att.Value;
                nd.Attributes.Append(at);
            }

            if (node.ChildNodes != null)
            foreach (XmlNode child in node.ChildNodes)
            {
                nd.AppendChild(ConvertXmlElementToLowerCase(xmldoc, child));
            }

            return nd;
        }

        private string ConvertXMLToLowerCase(string xml)
        {
            XmlDocument xmldoc = new XmlDocument();
            try
            {
                xml = xml.Replace("&", "_A_N_D_");
                xmldoc.LoadXml(xml);
                if (xmldoc.FirstChild.NodeType != XmlNodeType.XmlDeclaration)
                    xml = ConvertXmlElementToLowerCase(xmldoc, xmldoc.FirstChild).OuterXml;
                else
                    xml = ConvertXmlElementToLowerCase(xmldoc, xmldoc.LastChild).OuterXml;
            }
            catch(Exception ex)
            {

            }

            return xml;
        }

        private string GetOrders(DateTime startdate, DateTime enddate)
        {
            DateTime dt = startdate;

            string orders_page = ServiceProvider.RequiredParams["orders_page"].ToString();

            string xml = "";

            while (dt <= enddate)
            {
                string res = Get(orders_page + "?date=" + dt.ToString("dd.MM.yyyy"));
                if (res.Length > 0)
                {
                    if (dt != startdate)
                        xml += "[{(XML_SEP)}]";
                    res = ConvertXMLToLowerCase(res);
                    xml += StringCompressor.CompressString(res);
                }
                else
                    if (res.Contains("error_str"))
                    {
                        AddLog("GetOrders - [Get method failed]", true);
                        return "";
                    }

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

            Hashtable dates = new Hashtable();

            XmlDocument xmldoc = new XmlDocument();

            string[] xmls = res.Split(new string[1] { "[{(XML_SEP)}]" }, StringSplitOptions.None);

            //string[] dxmls = new string[xmls.Length];
            Hashtable dxmls = new Hashtable();

            for (int m = 0; m < xmls.Length; m++)
            {
                string xmlstr = StringCompressor.DecompressString(xmls[m]);
                if (xmlstr.Contains("error_str"))
                {
                    AddLog("GetOrders", true);
                    return null;
                }
                try
                {
                    xmldoc.LoadXml(xmlstr);
                }
                catch(Exception ex)
                {
                    AddLog("GetOrders - [Error loading XML][" + ex.Message + "]", true);
                    return null;
                }

                XmlElement root = xmldoc["orders"];

                if (root == null)
                {
                    AddLog("GetOrders - [Root element 'orders' is missing]", true);
                    return null;
                }

                foreach (XmlElement node in root.ChildNodes)
                {
                    if (node.Name.ToLower() == "order")
                    {
                        XmlAttribute id = node.Attributes["id"];
                        if (id == null)
                        {
                            AddLog("GetOrders - [Attribute 'id' is missing]", true);
                            return null;
                        }

                        XmlNode status = node["status"];
                        XmlNode datetodeliver = node["datetodeliver"];
                        XmlNode deliverycost = node["deliverycost"];
                        XmlNode deliveryprice = node["deliveryprice"];
                        XmlNode iscash = node["iscash"];
                        XmlNode additionaldeliverysum = node["additionaldeliverysum"];
                        XmlNode sum = node["sum"];
                        XmlNode discounted = node["discounted"];
                        XmlNode ordersource = node["ordersource"];
                        XmlNode deliverycity = node["deliverycity"];
                        XmlNode orderdate = node["dateordered"];

                        if (status == null || datetodeliver == null || deliverycost == null || deliveryprice == null || iscash == null || additionaldeliverysum == null || sum == null || discounted == null)
                        {
                            AddLog("GetOrders - [One of the fields is missing]", true);
                            return null;
                        }

                        string details = GetOrderDetails(Convert.ToInt32(id.Value));

                        if (m == 190) 
                            m = 190;

                        details = ConvertXMLToLowerCase(details);

                        dxmls[id.Value] = StringCompressor.CompressString(details);

                        XmlDocument dxml = new XmlDocument();

                        try
                        {
                            dxml.LoadXml(details);
                        }
                        catch(Exception ex)
                        {
                            AddLog("GetOrders - [Error loading details XML][" + ex.Message + "]", true);
                            return null;
                        }

                        XmlElement droot = dxml["order"];

                        if (orderdate == null)
                            orderdate = droot["dateordered"];

                        if (orderdate == null)
                        {
                            AddLog("GetOrders - [Field 'dateordered' in details XML is missing]", true);
                            return null;
                        }

                        XmlElement products = droot["orderedproducts"];

                        if (products == null)
                        {
                            AddLog("GetOrders - [Element 'orderedproducts' in details XML is missing]", true);
                            return null;
                        }

                        foreach (XmlNode node1 in products.ChildNodes)
                        {
                            if (node1.Name.ToLower() != "product") continue;

                            XmlAttribute pid = node1.Attributes["id"];
                            XmlNode ProductTitle = node1["producttitle"];
                            XmlNode ProductStatus = node1["productstatus"];
                            XmlNode Quantity = node1["quantity"];
                            XmlNode SingleProductPrice = node1["singleproductprice"];
                            XmlNode TotalSum = node1["totalsum"];
                            XmlNode DiscountedPrice = node1["discountedprice"];

                            if (pid == null || ProductTitle == null || ProductStatus == null || Quantity == null || SingleProductPrice == null || TotalSum == null || DiscountedPrice == null)
                            {
                                AddLog("GetOrders - [One of the fields in details XML is missing]", true);
                                return null;
                            }
                        }

                        products = droot["returnedproducts"];

                        if (products == null)
                        {
                            AddLog("GetOrders - [Element 'returnedproducts' in details XML is missing]", true);
                            return null;
                        }

                        foreach (XmlNode node1 in products.ChildNodes)
                        {
                            if (node1.Name.ToLower() != "product") continue;

                            XmlAttribute pid = node1.Attributes["id"];
                            XmlNode ProductTitle = node1["producttitle"];
                            XmlNode Quantity = node1["quantity"];
                            XmlNode TotalSum = node1["totalsum"];

                            if (pid == null || ProductTitle == null || Quantity == null || TotalSum == null)
                            {
                                AddLog("GetOrders - [One of the fields in details XML is missing]", true);
                                return null;
                            }
                        }
                    }
                }
            }

            //************************

            for (int m = 0; m < xmls.Length; m++)
            {
                string xmlstr = StringCompressor.DecompressString(xmls[m]);
                try
                {
                    xmldoc.LoadXml(xmlstr);
                }
                catch
                {
                    continue;
                }

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
                        XmlNode ordersource = node["ordersource"];
                        XmlNode deliverycity = node["deliverycity"];
                        XmlNode orderdate = node["dateordered"];

                        if (status == null || datetodeliver == null || deliverycost == null || deliveryprice == null || iscash == null || additionaldeliverysum == null || sum == null || discounted == null)
                            continue;

                        DateTime dt = DateTime.Now;
                        if (!DateTime.TryParse(datetodeliver.InnerText, out dt))
                            dt = DateTime.Now;

                        //string details = GetOrderDetails(Convert.ToInt32(id.Value));

                        //details = ConvertXMLToLowerCase(details);

                        string details = StringCompressor.DecompressString(dxmls[id.Value].ToString());

                        XmlDocument dxml = new XmlDocument();

                        try
                        {
                            dxml.LoadXml(details);
                        }
                        catch
                        {
                            continue;
                        }

                        XmlElement droot = dxml["order"];

                        if  (orderdate == null)
                            orderdate = droot["dateordered"];

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

                        string dcity = "";
                        if (deliverycity != null)
                            dcity = deliverycity.InnerText;
                        string osource = "";
                        if (ordersource != null)
                            osource = ordersource.InnerText;

                        if (dates[datetime.Year.ToString() + datetime.Month.ToString() + datetime.Day.ToString()] == null)
                        {
                            DBModule.DeleteOrders(SyncManager.CurExecProject.DbId, datetime);
                            dates[datetime.Year.ToString() + datetime.Month.ToString() + datetime.Day.ToString()] = true;
                        }

                        int oid = DBModule.AddOrder(Convert.ToInt32(id.Value), status.InnerText.Replace("_A_N_D_", "&"), datetime, dt, dc, dp, Convert.ToBoolean(iscash.InnerText), ads, sm, ds, ServiceProvider.Name, SyncManager.CurExecProject.DbId, osource.Replace("_A_N_D_", "&"), dcity.Replace("_A_N_D_", "&"));

                        if (oid <= 0)
                        {
                            if (oid == 0)
                            {
                                AddLog("GetOrders(AddOrder) - [" + DBModule.lastErrorString + "]", true);
                                DBModule.DeleteOrders(SyncManager.CurExecProject.DbId, datetime);
                                return null;
                            }
                            continue;
                        }

                        XmlElement products = droot["orderedproducts"];

                        if (products == null) continue;

                        foreach (XmlNode node1 in products.ChildNodes)
                        {
                            if (node1.Name.ToLower() != "product") continue;

                            XmlAttribute pid = node1.Attributes["id"];
                            XmlNode ProductTitle = node1["producttitle"];
                            XmlNode ProductStatus = node1["productstatus"];
                            XmlNode Quantity = node1["quantity"];
                            XmlNode SingleProductPrice = node1["singleproductprice"];
                            XmlNode TotalSum = node1["totalsum"];
                            XmlNode DiscountedPrice = node1["discountedprice"];

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


                            if (!DBModule.AddOrderDetails(oid, Convert.ToInt32(pid.Value), ProductTitle.InnerText.Replace("_A_N_D_", "&"), ProductStatus.InnerText.Replace("_A_N_D_", "&"), (float)qty, spp, ts, dp, false))
                            {
                                AddLog("GetOrders(AddOrderDetails) - [" + DBModule.lastErrorString + "]", true);
                                DBModule.DeleteOrders(SyncManager.CurExecProject.DbId, datetime);
                                return null;
                            }
                        }

                        products = droot["returnedproducts"];

                        if (products == null) continue;

                        foreach (XmlNode node1 in products.ChildNodes)
                        {
                            if (node1.Name.ToLower() != "product") continue;

                            XmlAttribute pid = node1.Attributes["id"];
                            XmlNode ProductTitle = node1["producttitle"];
                            XmlNode Quantity = node1["quantity"];
                            XmlNode TotalSum = node1["totalsum"];

                            if (pid == null || ProductTitle == null || Quantity == null || TotalSum == null) continue;


                            double qty = Convert.ToDouble(ChangeDecimalSymbol(Quantity.InnerText));
                            decimal ts = Convert.ToDecimal(ChangeDecimalSymbol(TotalSum.InnerText));

                            if (!DBModule.AddOrderDetails(oid, Convert.ToInt32(pid.Value), ProductTitle.InnerText, "", (float)qty, 0, ts, 0, true))
                            {
                                AddLog("GetOrders(AddOrderDetails) - [" + DBModule.lastErrorString + "]", true);
                                DBModule.DeleteOrders(SyncManager.CurExecProject.DbId, datetime);
                                return null;
                            }
                        }

                    }
                }

            }

            dates.Clear();

            APIServiceProviderNamespace.main.ordersDataTable ndo = DBModule.GetNotDeliveredOrders();
            
            foreach (APIServiceProviderNamespace.main.ordersRow or in ndo.Rows)
            {
                string details = GetOrderDetails(or.oid);

                details = ConvertXMLToLowerCase(details);

                XmlDocument dxml = new XmlDocument();
                try
                {
                    dxml.LoadXml(details);
                }
                catch
                {
                    continue;
                }

                XmlElement droot = dxml["order"];
                if (droot == null) continue;

                XmlElement status = droot["Status"];
                if (status == null) continue;

                /*if (dates[or.dateordered.Year.ToString() + or.dateordered.Month.ToString() + or.dateordered.Day.ToString() + or.projectid.ToString()] == null)
                {
                    DBModule.DeleteOrders(or.projectid, or.dateordered);
                    dates[or.dateordered.Year.ToString() + or.dateordered.Month.ToString() + or.dateordered.Day.ToString() + or.projectid.ToString()] = true;
                }*/

                if (or.status == status.InnerText) continue;

                int oid = DBModule.AddOrder(or.oid, status.InnerText, or.dateordered, or.datetodeliver, or.deliverycost, or.deliveryprice, or.iscash, or.additionaldeliverysum, or.sum, or.discounted, or.source, or.projectid, or.ordersource, or.deliverycity);

                XmlElement products = droot["returnedproducts"];

                if (products == null) continue;

                if (oid < 0) oid = -oid;

                foreach (XmlNode node1 in products.ChildNodes)
                {
                    if (node1.Name.ToLower() != "product") continue;

                    XmlAttribute pid = node1.Attributes["id"];
                    XmlNode ProductTitle = node1["producttitle"];
                    XmlNode Quantity = node1["quantity"];
                    XmlNode TotalSum = node1["totalsum"];

                    if (pid == null || ProductTitle == null || Quantity == null || TotalSum == null) continue;


                    double qty = Convert.ToDouble(ChangeDecimalSymbol(Quantity.InnerText));
                    decimal ts = Convert.ToDecimal(ChangeDecimalSymbol(TotalSum.InnerText));

                    if (!DBModule.AddOrderDetails(oid, Convert.ToInt32(pid.Value), ProductTitle.InnerText, "", (float)qty, 0, ts, 0, true))
                    {
                        //AddLog("GetOrders(AddOrderDetails) - [" + DBModule.lastErrorString + "]", true);
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
