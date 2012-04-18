using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Xml;
using System.Net;
using System.Reflection;
using System.IO;

namespace APIServiceProviderNamespace
{
    public class APIServiceProvider
    {
        #region Private Members

        private string name = "";
        private string url = "";
        private Hashtable requiredParams = new Hashtable();
        private APIServiceProviderRequest serviceProviderRequest = new APIServiceProviderRequest();

        private Project proj = null;

        Hashtable formats = new Hashtable();

        #endregion

        #region Public Members

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        public string Url
        {
            get { return url; }
            set { url = value; }
        }

        public Hashtable RequiredParams
        {
            get { return requiredParams; }
         // set { requiredParams = value; }
        }

        public virtual APIServiceProviderRequest ServiceProviderRequest
        {
            get { return serviceProviderRequest; }
            set { serviceProviderRequest = value; }
        }

        public Project Proj
        {
            get { return proj; }
            set { proj = value; }
        }

        public Hashtable Formats
        {
            get { return formats; }
            set { formats = value; }
        }

        #endregion

        #region Public Methods

        public virtual string GetQueryString()
        {
            return "";
        }

        #endregion

        #region Protected Methods


        #endregion

        public APIServiceProvider()
        {

        }
    }

    public class APIServiceProviderCollection
    {
        #region Private Members

        Hashtable serviceProviders = new Hashtable();

        #endregion

        #region Public Members

        public Hashtable ServiceProviders
        {
            get { return serviceProviders; }
            set { serviceProviders = value; }
        }

        #endregion

        #region Public Methods

        public bool LoadFromXML(string filename)
        {
            if (!System.IO.File.Exists(filename)) return false;

            XmlDocument xmldoc = new XmlDocument();
            xmldoc.Load(filename);

            XmlElement root = xmldoc["providers"];

            if (root == null) return false;

            ServiceProviders.Clear();

            foreach (XmlNode node in root.ChildNodes)
            {
                if (node.Name.ToLower() != "provider") return false;

                XmlNode typenode = node["type"];
                XmlNode namenode = node["name"];
                XmlNode urlnode = node["url"];
                XmlNode paramsnode = node["params"];

                if (typenode == null || namenode == null || urlnode == null || paramsnode == null) return false;

                XmlElement formats = node["formats"];

                string type = typenode.InnerText;
                string name = namenode.InnerText;
                string url = urlnode.InnerText;

                Type t = Type.GetType("APIServiceProviderNamespace." + type);

                if (t != null)
                {
                    APIServiceProvider instance = (APIServiceProvider)Activator.CreateInstance(t);
                    instance.Name = name;
                    instance.Url = url;

                    if (formats != null)
                    {
                        foreach (XmlNode f in formats.ChildNodes)
                        {
                            instance.Formats[f.Name] = f.InnerText;
                        }
                    }

                    
                    foreach (XmlNode node1 in paramsnode.ChildNodes)
                    {
                        if (node1.Name.ToLower() != "param") return false;

                        XmlNode paramname = node1["name"];
                        XmlNode paramvalue = node1["value"];

                        if (paramname == null || paramvalue == null) return false;

                        if (paramname.InnerText.Length > 0)
                            instance.RequiredParams[paramname.InnerText] = paramvalue.InnerText;
                    }

                    ServiceProviders[name] = instance;
                }
            }

            return true;
        }


        #endregion

        public APIServiceProviderCollection()
        {
            string path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().CodeBase);
            if (System.IO.File.Exists(path.Replace("file:\\", "") + "\\config.xml"))
                LoadFromXML(path.Replace("file:\\", "") + "\\config.xml");
        }
    }

    public class YandexAPIServiceProvider : APIServiceProvider
    {
        #region Public Methods

        public override string  GetQueryString()
        {
            return "\"token\":\"" + RequiredParams["token"].ToString() + "\", \"application_id\":\"" + RequiredParams["application_id"].ToString() + "\", \"login\":\"" + RequiredParams["login"].ToString() + "\"";
        }

        #endregion


        public YandexAPIServiceProvider()
        {
            ServiceProviderRequest = new YandexAPIServiceProviderRequest();
            ServiceProviderRequest.ServiceProvider = this;
            string[] str = new string[1];
            str[0] = "4447250";

            //string res = ServiceProviderRequest.HandleRequest("getsummarystats", str, DateTime.Now.AddDays(-3), DateTime.Now);
            //string res = ServiceProviderRequest.HandleRequest("getcampaignslist", "fitmedia");
        }
    }

    public class OrdersAPIServiceProvider : APIServiceProvider
    {
        #region Public Methods

        public override string GetQueryString()
        {
            return "";
        }

        #endregion


        public OrdersAPIServiceProvider()
        {
            ServiceProviderRequest = new OrdersAPIServiceProviderRequest();
            ServiceProviderRequest.ServiceProvider = this;
        }
    }
}
