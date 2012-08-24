using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Collections;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Globalization;

namespace APIServiceProviderNamespace
{
   public class ProjectScheduleRequestParameter
   {
       #region Private Members
       
       private string name = "";
       private string type = "";
       private string value = "";
       private string format = "";

       #endregion

        #region Public Members

        public string Value
        {
          get { return this.value; }
          set { this.value = value; }
        }

        public string Type
        {
          get { return type; }
          set { type = value; }
        }

        public string Name
        {
          get { return name; }
          set { name = value; }
        }

        public string Format
        {
            get { return format; }
            set { format = value; }
        }

        #endregion

       #region Public Methods

        public void LoadFromXML(XmlNode node)
        {
            if (node["name"] != null && node["type"] != null && node["value"] != null)
            {
                Name = node["name"].InnerText;
                Type = node["type"].InnerText;
                Value = node["value"].InnerText;
                if (node["format"] != null)
                    Format = node["format"].InnerText;
            }
        }

       #endregion
   }

   public class ProjectScheduleParameterCollection
   {
       #region Private Region

       private ArrayList items = new ArrayList();

       #endregion

       #region Public Region

       public ArrayList Items
       {
           get { return items; }
       }

       #endregion

       public ProjectScheduleParameterCollection()
       {
       }
   }

    public class ProjectScheduleRequest
    {
        #region Private Region

        private string name = "";
        private ProjectScheduleParameterCollection parameters = new ProjectScheduleParameterCollection();
        ProjectSchedule schedule = null;
        bool isInitial = false;

        #endregion

        #region Public Members

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        public ProjectScheduleParameterCollection Parameters
        {
            get { return parameters; }
            set { parameters = value; }
        }

        public ProjectSchedule Schedule
        {
            get { return schedule; }
            set { schedule = value; }
        }

        public bool IsInitial
        {
            get { return isInitial; }
            set { isInitial = value; }
        }

        #endregion

        #region Public Methods

        public bool LoadFromXML(XmlNode node)
        {
            if (node.Name == "request" && node["name"] != null)
            {
                Name = node["name"].InnerText;
                bool isinitial = false;
                if (node["isinitial"] != null)
                    if (!bool.TryParse(node["isinitial"].InnerText, out isinitial))
                        isinitial = false;
                IsInitial = isinitial;
                if (node["params"] != null)
                {
                    XmlElement prs = node["params"];

                    foreach (XmlNode param in prs.ChildNodes)
                    {
                        ProjectScheduleRequestParameter psrp = new ProjectScheduleRequestParameter();
                        psrp.LoadFromXML(param);
                        Parameters.Items.Add(psrp);
                    }
                }

                return true;
            }

            return false;
        }

        public void LogFailedRequest(string logxml)
        {
            XmlDocument xmldoc = new XmlDocument();

            string path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().CodeBase);
            if (!System.IO.File.Exists(path.Replace("file:\\", "") + "\\failed.xml"))
            {
                xmldoc.LoadXml("<?xml version=\"1.0\" encoding=\"utf-8\" ?><requests></requests>");
            }
            else
                xmldoc.Load(path.Replace("file:\\", "") + "\\failed.xml");

            XmlElement root = xmldoc["requests"];

            if (root != null)
            {
                root.InnerXml += logxml;
            }

            xmldoc.Save(path.Replace("file:\\", "") + "\\failed.xml");
        }

        private object[] Validate()
        {
            if (parameters.Items.Count == 0) return null;

            object[] prs = new object[parameters.Items.Count + 1];
            prs[0] = name;

            for (int i = 0; i < parameters.Items.Count; i++)
            {
                ProjectScheduleRequestParameter p = (parameters.Items[i] as ProjectScheduleRequestParameter);

                switch (p.Type)
                {
                    case "int":
                        {
                            int res = 0;
                            if (!int.TryParse(p.Value, out res))
                            {
                                return null;
                            }
                            prs[i + 1] = res;
                        }
                        break;
                    case "float":
                        {
                            float res = 0;
                            if (!float.TryParse(p.Value, out res))
                            {
                                return null;
                            }
                            prs[i + 1] = res;
                        }
                        break;
                    case "datetime":
                        {
                            DateTime res = SyncManager.ExecutingDate;
                            if (p.Format.Length == 0)
                                p.Format = "dd.MM.yyyy";
                            try
                            {
                                if (p.Value.Length > 0)
                                    res = DateTime.ParseExact(p.Value, p.Format, null);
                            }
                            catch (Exception exobj)
                            {
                                return null;
                            }
                            prs[i + 1] = res;
                        }
                        break;
                    case "boolean":
                        {
                            bool res = false;
                            if (!bool.TryParse(p.Value, out res))
                            {
                                return null;
                            }
                            prs[i + 1] = res;
                        }
                        break;
                    default:
                        prs[i + 1] = p.Value;
                        break;
                }
            }

            return prs;
        }

        public string Request()
        {
            if (schedule == null) return "";
            if (schedule.Provider == null) return "";

            object[] prs = Validate();
            if (prs == null) return "";

            schedule.Provider.ServiceProviderRequest.IsInitial = IsInitial;

            schedule.Provider.ServiceProviderRequest.HandleRequest(prs);

            if (schedule.Provider.ServiceProviderRequest.HasError)
                LogFailedRequest(schedule.Provider.ServiceProviderRequest.RequestLogXml);

            return schedule.Provider.ServiceProviderRequest.RequestLogXml;
        }

        #endregion

        public ProjectScheduleRequest()
        {

        }
    }

    public class ProjectScheduleRequestCollection
    {
       #region Private Region

       private ArrayList items = new ArrayList();

       #endregion

       #region Public Region

       public ArrayList Items
       {
           get { return items; }
       }

       #endregion

       public ProjectScheduleRequestCollection()
       {
       }
    }

    public class ProjectSchedule
    {
        #region Private Members

        private APIServiceProvider provider = null;
        private DateTime startAt = DateTime.Now;
        private DateTime lastExecutedAt = DateTime.Now;

        private string interval = "daily";
        private ProjectScheduleRequestCollection requests = new ProjectScheduleRequestCollection();
        private bool initalRequestDone = false;
        private Project parentProject = null;

        private string id = "";

        #endregion

        #region Public Members

        public APIServiceProvider Provider
        {
            get { return provider; }
            set { provider = value; }
        }

        public DateTime StartAt
        {
            get { return startAt; }
            set { startAt = value; }
        }

        public DateTime LastExecutedAt
        {
            get { return lastExecutedAt; }
            set { lastExecutedAt = value; }
        }

        public string Interval
        {
            get { return interval; }
            set { interval = value; }
        }

        public ProjectScheduleRequestCollection Requests
        {
            get { return requests; }
        }

        public Project ParentProject
        {
            get { return parentProject; }
            set { parentProject = value; }
        }

        public string Id
        {
            get { return id; }
            set { id = value; }
        }

        #endregion

        #region Private Methods
        #endregion

        #region Public Methods

        public bool LoadFromXML(XmlNode node)
        {
            XmlNode scid = node["id"];
            XmlNode provider = node["provider"];
            XmlNode start = node["start"];
            XmlNode interval = node["interval"];

            if (provider == null) return false;

            DateTime st = DateTime.Now;
            if (!DateTime.TryParse(start.InnerText, out st))
                st = DateTime.Now;

            string it = "daily";
            if (interval.InnerText == "daily" ||
                interval.InnerText == "hourly" ||
                interval.InnerText == "weekly")
                it = interval.InnerText;

            Provider = (APIServiceProvider)SyncManager.Providers.ServiceProviders[provider.InnerText];
            Interval = it;
            StartAt = st;
            //LastExecutedAt = st;
            if (scid != null)
                Id = scid.InnerText;

            XmlElement requests = node["requests"];

            if (requests != null)
            {
                foreach (XmlNode nd in requests.ChildNodes)
                {
                    ProjectScheduleRequest psr = new ProjectScheduleRequest();
                    psr.Schedule = this;

                    if (psr.LoadFromXML(nd))
                        Requests.Items.Add(psr);
                }
            }

            return true;
        }

        private void InitialCheck()
        {
            string path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().CodeBase);
            if (System.IO.File.Exists(path.Replace("file:\\", "") + "\\log.xml"))
            {
                XmlDocument xmldoc = new XmlDocument();
                xmldoc.Load(path.Replace("file:\\", "") + "\\log.xml");

                XmlElement root = xmldoc["globallogs"];
                if (root == null) return;

                foreach (XmlNode node in root.ChildNodes)
                {
                    if (node.Name == "globallog")
                    {
                        XmlAttribute dt = node.Attributes["datetime"];
                        foreach (XmlNode node1 in node.ChildNodes)
                        {
                            if (node1.Name == "projectlog")
                            {
                                XmlAttribute att = node1.Attributes["name"];
                                if (att != null && att.Value == ParentProject.Name)
                                {
                                    if (node1.ChildNodes.Count == 0) continue;
                                    foreach (XmlNode node2 in node1.ChildNodes)
                                    {
                                        if (node2.Name == "schedulelog")
                                        {
                                            att = node2.Attributes["id"];
                                            XmlAttribute att1 = node2.Attributes["initial"];
                                            bool inital = false;
                                            if (att1 != null && att1.Value == "True") inital = true;
                                            if (att != null && att.Value == id)
                                            {
                                                if (!initalRequestDone)
                                                    initalRequestDone = inital;
                                                if (dt != null)
                                                {
                                                    try
                                                    {
                                                        LastExecutedAt = DateTime.ParseExact(dt.Value, "dd.MM.yyyy HH:mm:ss", null);
                                                    }
                                                    catch
                                                    {
                                                        try
                                                        {
                                                            LastExecutedAt = DateTime.ParseExact(dt.Value, "dd.MM.yyyy HH:mm", null);
                                                        }
                                                        catch
                                                        {
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        public string Execute()
        {
            if (provider == null) return "";

            DateTime now = SyncManager.ExecutingDate;
            string logxml = "<schedulelog datetime=\"" + now.ToString("dd.MM.yyyy HH:mm") + "\" id=\"" + id + "\" initial=\"";

            if (!initalRequestDone)
                InitialCheck();

            if (!initalRequestDone)
            {
                //if (DateTime.Now >= StartAt)
                {
                    initalRequestDone = true;

                    logxml += "True\">";

                    for (int i = 0; i < requests.Items.Count; i++)
                    {
                        ProjectScheduleRequest psr = (requests.Items[i] as ProjectScheduleRequest);
                        if (psr.IsInitial)
                        {
                            logxml += psr.Request();
                        }
                    }
                }

                DateTime dt = new DateTime(SyncManager.ExecutingDate.Year, SyncManager.ExecutingDate.Month, SyncManager.ExecutingDate.Day, 23, 59, 59);//DateTime.Now;
                LastExecutedAt = dt.AddDays(-1);

                logxml += "</schedulelog>";

                return logxml;
            }

            logxml += "False\">";

            TimeSpan ts = SyncManager.ExecutingDate - LastExecutedAt;

            //if (!SyncManager.IsProperlyShutedDown())
            {
                SyncManager.ExecutingDate = SyncManager.ExecutingDate.AddDays(-1);

                int days = ts.Days;

                while (days > 0)
                {
                    for (int i = 0; i < requests.Items.Count; i++)
                    {
                        ProjectScheduleRequest psr = (requests.Items[i] as ProjectScheduleRequest);
                        if (!psr.IsInitial)
                            psr.Request();
                    }

                    SyncManager.ExecutingDate = SyncManager.ExecutingDate.AddDays(-1);
                    days--;
                }

                SyncManager.ExecutingDate = now;
            }

            try
            {
                string path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().CodeBase);
                File.AppendAllText(path.Replace("file:\\", "") + "\\requests1.txt", this.id.ToString() + ";" + SyncManager.ExecutingDate.ToString() + ";" + LastExecutedAt.ToString() + ";" + ts.TotalHours.ToString() + "\r\n");
            }
            catch
            {
            }

            switch (interval)
            {
                case "hourly":
                    if (ts.TotalHours < 0.995) return "";
                    break;
                case "daily":
                    if (ts.TotalDays < 0.995) return "";
                    break;
                case "weekly":
                    if (ts.TotalDays < 6.995) return "";
                    break;
            }

            for (int i = 0; i < requests.Items.Count; i++)
            {
                ProjectScheduleRequest psr = (requests.Items[i] as ProjectScheduleRequest);
                if (!psr.IsInitial)
                    logxml += psr.Request();
            }

            LastExecutedAt = new DateTime(SyncManager.ExecutingDate.Year, SyncManager.ExecutingDate.Month, SyncManager.ExecutingDate.Day, SyncManager.ExecutingDate.Hour, SyncManager.ExecutingDate.Minute, 59);

            logxml += "</schedulelog>";

            return logxml;
        }

        #endregion

        public ProjectSchedule()
        {
            DateTime dt = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 23, 59, 59);
            LastExecutedAt = dt.AddDays(-1);
        }
    }

    public class ProjectScheduleCollection
    {
        #region Private Region

        private ArrayList items = new ArrayList();

        #endregion

        #region Public Region

        public ArrayList Items
        {
            get { return items; }
        }

        #endregion

        public ProjectScheduleCollection()
        {
        }
    }


    public class ProjectCoef
    {
        #region Private members

        private float coefValue = 1;
        private DateTime coefDate = DateTime.Now;

        private string providerName = "";

        private Project coefProject = null;

        #endregion

        #region Public Region

        public float CoefValue
        {
            get { return coefValue; }
            set { coefValue = value; }
        }

        public DateTime CoefDate
        {
            get { return coefDate; }
            set { coefDate = value; }
        }

        public string ProviderName
        {
            get { return providerName; }
            set { providerName = value; }
        }

        public Project CoefProject
        {
            get { return coefProject; }
            set { coefProject = value; }
        }

        #endregion

        #region Public Methods

        public bool LoadFromXML(XmlNode node)
        {
            XmlAttribute att = node.Attributes["provider"];
            if (att != null)
            {
                if (SyncManager.Providers.ServiceProviders[att.Value] != null)
                {
                    ProviderName = att.Value;

                    XmlElement val = node["value"];
                    XmlElement dt = node["date"];

                    if (val != null)
                    {
                        try
                        {
                            CoefValue = float.Parse(val.InnerText);
                        }
                        catch
                        {
                            CoefValue = 1;
                        }
                    }

                    if (dt != null)
                    {
                        try
                        {
                            CoefDate = DateTime.ParseExact(dt.InnerText, "dd.MM.yyyy", null);
                        }
                        catch
                        {
                            CoefDate = DateTime.Now;
                        }
                    }

                    return true;
                }
            }

            return false;
        }

        #endregion

        public ProjectCoef()
        {
        }
    }


    public class Project
    {
        #region Private Region

        private string name = "";
        private string id = "";
        private int dbId = 0;
        private string xmlFileName = "";

        private ProjectScheduleCollection schedules = new ProjectScheduleCollection();
        SyncManager manager = null;

        DateTime startFrom = DateTime.Now;

        Hashtable coefs = new Hashtable();

        #endregion

        #region Public Members

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        public string Id
        {
            get { return id; }
            set { id = value; }
        }

        public ProjectScheduleCollection Schedules
        {
            get { return schedules; }
        }

        public int DbId
        {
            get { return dbId; }
            set { dbId = value; }
        }

        public string XmlFileName
        {
            get { return xmlFileName; }
            set { xmlFileName = value; }
        }

        public SyncManager Manager
        {
            get { return manager; }
            set { manager = value; }
        }

        public DateTime StartFrom
        {
            get { return startFrom; }
            set { startFrom = value; }
        }

        public Hashtable Coefs
        {
            get { return coefs; }
            set { coefs = value; }
        }

        public enum LoadOptions
        {
            All = 1,
            Coefs = 2,
            Schedules = 3
        }

        #endregion

        #region Public Methods

        public bool LoadFromXML(string filename, LoadOptions lo)
        {
            if (!System.IO.File.Exists(filename)) return false;

            XmlDocument xmldoc = new XmlDocument();
            xmldoc.Load(filename);

            XmlElement root = xmldoc["project"];

            if (root == null) return false;

            if (root["name"] == null || root["id"] == null) return false;

            name = root["name"].InnerText;
            id = root["id"].InnerText;

            if (root["startfrom"] != null)
            {
                try
                {
                    startFrom = DateTime.ParseExact(root["startfrom"].ToString(), "dd.MM.yyyy", null);
                }
                catch
                {
                    startFrom = DateTime.Now;
                }
            }

            XmlElement cfs = root["coefs"];

            if (cfs != null && (lo == LoadOptions.All || lo == LoadOptions.Coefs))
            {
                coefs.Clear();
                foreach (XmlNode cf in cfs.ChildNodes)
                {
                    ProjectCoef pc = new ProjectCoef();
                    pc.CoefProject = this;
                    if (pc.LoadFromXML(cf))
                        coefs[pc.ProviderName] = pc;
                }
            }

            XmlElement schedules = root["schedules"];

            if (schedules == null) return false;

            if (lo == LoadOptions.All || lo == LoadOptions.Schedules)
            {
                Schedules.Items.Clear();
                foreach (XmlNode node in schedules.ChildNodes)
                {
                    if (node.Name.ToLower() != "schedule") continue;

                    ProjectSchedule ps = new ProjectSchedule();
                    ps.ParentProject = this;

                    if (ps.LoadFromXML(node))
                        Schedules.Items.Add(ps);
                }
            }

            return true;
        }

        public string Execute()
        {
            DateTime now = SyncManager.ExecutingDate;

            string logxml = "<projectlog datetime=\"" + now.ToString("dd.MM.yyyy HH:mm:ss") + "\" name=\"" + name + "\">";

           /*XmlElement root = SyncManager.ExecLogXml["execlogs"];

            if (root != null)
            {
                XmlElement execlog = SyncManager.ExecLogXml.CreateElement("execlog");
                XmlAttribute att = SyncManager.ExecLogXml.CreateAttribute("datetime");
                att.Value = now.ToShortDateString() + " " + now.ToShortTimeString();
            }*/

            int n = 0;

            for (int i = 0; i < schedules.Items.Count; i++)
            {
                string s = (schedules.Items[i] as ProjectSchedule).Execute();
                if (s.Length == 0) n++;
                logxml += s;
            }

            logxml += "</projectlog>";

            if (n == schedules.Items.Count)
                return "";

            return logxml;
        }

        #endregion

        public Project()
        {
        }
    }

    public class ProjectCollection
    {
        #region Private Region

        ArrayList items = new ArrayList();

        #endregion

        #region Public Members

        public ArrayList Items
        {
            get { return items; }
        }

        #endregion

        #region Public Methods

        public bool LoadProjects(SyncManager manager, Project.LoadOptions lo)
        {
            string path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().CodeBase);
            if (!System.IO.Directory.Exists(path.Replace("file:\\", "") + "\\projects")) return false;

            string[] files = System.IO.Directory.GetFiles(path.Replace("file:\\", "") + "\\projects", "*.xml");

            if (lo == Project.LoadOptions.All)
                items.Clear();

            for (int i = 0; i < files.Length; i++)
            {
                Project prj = null;
                if (lo == Project.LoadOptions.All)
                {
                    prj = new Project();
                    prj.XmlFileName = files[i];
                    items.Add(prj);
                }
                else
                    for (int j = 0; j < items.Count; j++)
                        if (((Project)items[j]).XmlFileName == files[i])
                            prj = (Project)items[j];

                if (prj == null)
                {
                    prj = new Project();
                    prj.XmlFileName = files[i];
                    items.Add(prj);
                }

                if (!prj.LoadFromXML(files[i], lo))
                {
                    items.Clear();
                    return false;
                }

                prj.Manager = manager;
                prj.DbId = DBModule.AddProject(prj.Name, prj.Id, prj.StartFrom);

                foreach(object k in prj.Coefs.Keys)
                {
                    ProjectCoef pc = (ProjectCoef)prj.Coefs[k.ToString()];
                    if (pc != null)
                    {
                        APIServiceProvider sp = (APIServiceProvider)SyncManager.Providers.ServiceProviders[pc.ProviderName];
                        DBModule.AddCoef(prj.DbId, pc.CoefDate, pc.CoefValue, sp.ServiceProviderType);
                    }
                }
            }

            return true;
        }

        #endregion
    }

    public class SyncManager
    {
        #region Private Region

        private ProjectCollection projects = new ProjectCollection();
        private static APIServiceProviderCollection providers = new APIServiceProviderCollection();
        private bool isInitialized = false;

        private static Project curExecProject = null;

        System.Timers.Timer timer = new System.Timers.Timer();
        System.Timers.Timer checkTimer = new System.Timers.Timer();

        private static XmlDocument execLogXml = new XmlDocument();

        private static DateTime executingDate = DateTime.Now;

        private bool isStoped = false;

        private bool isFirstTimerEvent = true;

        #endregion

        #region Public Members

        public static APIServiceProviderCollection Providers
        {
            get { return SyncManager.providers; }
            set { SyncManager.providers = value; }
        }

        public static Project CurExecProject
        {
            get { return SyncManager.curExecProject; }
            set { SyncManager.curExecProject = value; }
        }

        public static XmlDocument ExecLogXml
        {
            get { return execLogXml; }
        }

        public static DateTime ExecutingDate
        {
            get { return SyncManager.executingDate; }
            set { SyncManager.executingDate = value; }
        }

        #endregion

        #region Public Methods

        public void Init()
        {
            if (isInitialized) return;
            projects.LoadProjects(this, Project.LoadOptions.All);

            Execute();
            isInitialized = true;
        }

        public static bool IsProperlyShutedDown()
        {
            string path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().CodeBase);
            return !File.Exists(path.Replace("file:\\", "") + "\\shutdown");
        }

        public bool Start()
        {
            Init();

            timer.Enabled = true;
            timer.Start();

            isStoped = false;

            checkTimer.Elapsed += new System.Timers.ElapsedEventHandler(checkTimer_Elapsed);
            checkTimer.Enabled = true;
            checkTimer.Interval = 30000;
            checkTimer.Start();

            return true;
        }

        public void Resume()
        {
            isStoped = false;
            timer.Enabled = true;
            timer.Start();
        }

        public void Stop()
        {
            isStoped = true;
            timer.Enabled = false;
            timer.Stop();
        }

        public void ExecuteFailedRequests()
        {
            XmlDocument xmldoc = new XmlDocument();

            string path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().CodeBase);
            if (!File.Exists(path.Replace("file:\\", "") + "\\failed.xml")) return;
            xmldoc.Load(path.Replace("file:\\", "") + "\\failed.xml");

            XmlElement root = xmldoc["requests"];

            SyncManager.CurExecProject = new Project();


            ArrayList nodestoremove = new ArrayList();

            foreach (XmlNode node in root.ChildNodes)
            {
                if (node.Name != "requestlog") continue;

                XmlAttribute datetime = node.Attributes["datetime"];

                XmlElement provider = node["provider"];
                XmlElement parameters = node["params"];
                XmlElement providername = node["providername"];
                XmlElement project = node["project"];

                SyncManager.CurExecProject.DbId = int.Parse(project.InnerText);

                Type t = Type.GetType(provider.InnerText);

                XmlAttribute method = node.Attributes["methodname"];

                APIServiceProviderCollection c = new APIServiceProviderCollection();

                object[] pr = new object[parameters.ChildNodes.Count + 1];

                pr[0] = method.InnerText;

                int i = 1;

                foreach (XmlNode p in parameters.ChildNodes)
                {
                    XmlAttribute type = p.Attributes["type"];
                    t = Type.GetType(type.Value);

                    if (t == typeof(DateTime))
                        pr[i] = DateTime.ParseExact(p.InnerText, "dd.MM.yyyy", null);
                    else
                        if (t == typeof(int))
                            pr[i] = int.Parse(p.InnerText);
                        else
                            if (t == typeof(bool))
                                pr[i] = bool.Parse(p.InnerText);
                            else
                                pr[i] = p.InnerText;

                    i++;
                }

                APIServiceProvider instance = (APIServiceProvider)c.ServiceProviders[providername.InnerText];

                if (instance != null)
                {
                    instance.ServiceProviderRequest.HandleRequest(pr);
                    if (!instance.ServiceProviderRequest.HasError)
                    {
                        nodestoremove.Add(node);
                        try
                        {
                            //instance.ServiceProviderRequest.ExecutingMethodName = "getreport";
                            DateTime dt = DateTime.ParseExact(datetime.Value, "dd.MM.yyyy HH:mm:ss", null);
                            DBModule.DeleteExecLog(int.Parse(project.InnerText), dt, instance.ServiceProviderRequest.ExecutingMethodName);
                        }
                        catch
                        {
                        }
                    }
                }
            }

            
            if (nodestoremove.Count > 0)
            {
                for (int i = 0; i < nodestoremove.Count; i++)
                    root.RemoveChild((XmlNode)nodestoremove[i]);

                xmldoc.Save(path.Replace("file:\\", "") + "\\failed.xml");
            }
        }

        public string Execute()
        {
            //if (isStoped) return "";
            SyncManager.ExecutingDate = DateTime.Now;
            /*if (isFirstTimerEvent && (SyncManager.ExecutingDate.Second != 59 || SyncManager.ExecutingDate.Minute != 59))
            {
                isFirstTimerEvent = false;
                return "";
            }*/

            try
            {
                string path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().CodeBase);
                //if (File.Exists(path.Replace("file:\\", "") + "\\requests.txt"))
                File.AppendAllText(path.Replace("file:\\", "") + "\\requests.txt", SyncManager.ExecutingDate.ToString() + "\r\n");
            }
            catch
            {
            }

            Stop();

            projects.LoadProjects(this, Project.LoadOptions.Coefs);

           ExecuteFailedRequests();

           string logxml = "<globallog datetime=\"" + SyncManager.ExecutingDate.ToString("dd.MM.yyyy HH:mm:ss") + "\">";

            int j = 0;

            for (int i = 0; i < projects.Items.Count; i++)
            {
                SyncManager.CurExecProject = (projects.Items[i] as Project);
                string s = (projects.Items[i] as Project).Execute();
                if (s.Length == 0) j++;
                logxml += s;
            }

            logxml += "</globallog>";

            if (j != projects.Items.Count)
            {
                DBModule.RecalcStatCache();

                XmlDocument xmldoc = new XmlDocument();

                string path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().CodeBase);
                if (!System.IO.File.Exists(path.Replace("file:\\", "") + "\\log.xml"))
                {
                    xmldoc.LoadXml("<?xml version=\"1.0\" encoding=\"utf-8\" ?><globallogs></globallogs>");
                }
                else
                    xmldoc.Load(path.Replace("file:\\", "") + "\\log.xml");


                XmlElement root = xmldoc["globallogs"];

                if (root == null)
                {
                    root = xmldoc.CreateElement("globallogs");
                    xmldoc.AppendChild(root);
                }

                root.InnerXml += logxml;

                xmldoc.Save(path.Replace("file:\\", "") + "\\log.xml");
            }

            if (isFirstTimerEvent)
            {
                isFirstTimerEvent = false;
                while (SyncManager.ExecutingDate.Second != 58 || SyncManager.ExecutingDate.Minute != 59)
                {
                    SyncManager.ExecutingDate = DateTime.Now;
                    try
                    {
                        string path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().CodeBase);
                        //if (File.Exists(path.Replace("file:\\", "") + "\\requests.txt"))
                        File.AppendAllText(path.Replace("file:\\", "") + "\\requests.txt", SyncManager.ExecutingDate.ToString() + "\r\n");
                    }
                    catch
                    {
                    }
                }
            }

            Resume();

            return logxml;
        }

        #endregion

        #region Private Members

        private void timer_Elapsed(object sender, EventArgs e)
        {
            Execute();
        }

        private void checkTimer_Elapsed(object sender, EventArgs e)
        {
            if (!timer.Enabled && !isStoped)
            {
                Resume();
            }
        }

        #endregion

        public SyncManager()
        {
            timer.Elapsed += new System.Timers.ElapsedEventHandler(timer_Elapsed);
            timer.Enabled = false;
            //timer.Interval = 3600000;
            //timer.Interval = 60000;
            timer.Interval = 1000;
        }
    }
}
