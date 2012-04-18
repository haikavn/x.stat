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
                            DateTime res = DateTime.Now;
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

        public bool Request()
        {
            if (schedule == null) return false;
            if (schedule.Provider == null) return false;

            object[] prs = Validate();
            if (prs == null) return false;

            schedule.Provider.ServiceProviderRequest.HandleRequest(prs);

            return true;
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

        #endregion

        #region Private Methods
        #endregion

        #region Public Methods

        public bool Execute()
        {
            if (provider == null) return false;

            if (!initalRequestDone)
            {
                //if (DateTime.Now >= StartAt)
                {
                    initalRequestDone = true;

                    for (int i = 0; i < requests.Items.Count; i++)
                    {
                        ProjectScheduleRequest psr = (requests.Items[i] as ProjectScheduleRequest);
                        if (psr.IsInitial)
                            psr.Request();
                    }
                }

                LastExecutedAt = DateTime.Now;

                return true;
            }

            TimeSpan ts = DateTime.Now - LastExecutedAt;

            switch (interval)
            {
                case "hourly":
                    if (ts.TotalHours < 1) return false;
                    break;
                case "daily":
                    if (ts.TotalDays < 1) return false;
                    break;
                case "weekly":
                    if (ts.TotalDays < 7) return false;
                    break;
            }

            for (int i = 0; i < requests.Items.Count; i++)
            {
                ProjectScheduleRequest psr = (requests.Items[i] as ProjectScheduleRequest);
                if (!psr.IsInitial)
                    psr.Request();
            }

            LastExecutedAt = DateTime.Now;

            return true;
        }

        #endregion

        public ProjectSchedule()
        {

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



    public class Project
    {
        #region Private Region

        private string name = "";
        private string id = "";
        private int dbId = 0;
        private ProjectScheduleCollection schedules = new ProjectScheduleCollection();
        SyncManager manager = null;

        DateTime startFrom = DateTime.Now;

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

        #endregion

        #region Public Methods

        public bool LoadFromXML(string filename)
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

            XmlElement schedules = root["schedules"];

            if (schedules == null) return false;

            foreach (XmlNode node in schedules.ChildNodes)
            {
                if (node.Name.ToLower() != "schedule") return false;

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

                ProjectSchedule ps = new ProjectSchedule();
                ps.ParentProject = this;
                ps.Provider = (APIServiceProvider)SyncManager.Providers.ServiceProviders[provider.InnerText];
                ps.Interval = it;
                ps.StartAt = st;
                ps.LastExecutedAt = st;

                XmlElement requests = node["requests"];

                if (requests != null)
                {
                    foreach (XmlNode nd in requests.ChildNodes)
                    {
                        if (nd.Name == "request" && nd["name"] != null)
                        {
                            ProjectScheduleRequest psr = new ProjectScheduleRequest();
                            psr.Schedule = ps;
                            psr.Name = nd["name"].InnerText;
                            bool isinitial = false;
                            if (nd["isinitial"] != null)
                                if (!bool.TryParse(nd["isinitial"].InnerText, out isinitial))
                                    isinitial = false;
                            psr.IsInitial = isinitial;
                            if (nd["params"] != null)
                            {
                                XmlElement prs = nd["params"];

                                foreach (XmlNode param in prs.ChildNodes)
                                {
                                    if (param["name"] != null && param["type"] != null && param["value"] != null)
                                    {
                                        ProjectScheduleRequestParameter psrp = new ProjectScheduleRequestParameter();
                                        psrp.Name = param["name"].InnerText;
                                        psrp.Type = param["type"].InnerText;
                                        psrp.Value = param["value"].InnerText;
                                        if (param["format"] != null)
                                            psrp.Format = param["format"].InnerText;
                                        psr.Parameters.Items.Add(psrp);
                                    }
                                }
                            }

                            ps.Requests.Items.Add(psr);
                        }
                    }
                }

                Schedules.Items.Add(ps);
            }

            return true;
        }

        public bool Execute()
        {
            for (int i = 0; i < schedules.Items.Count; i++)
            {
                (schedules.Items[i] as ProjectSchedule).Execute();
            }

            return true;
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

        public bool LoadProjects(SyncManager manager)
        {
            string path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().CodeBase);
            if (!System.IO.Directory.Exists(path.Replace("file:\\", "") + "\\projects")) return false;

            string[] files = System.IO.Directory.GetFiles(path.Replace("file:\\", "") + "\\projects", "*.xml");

            items.Clear();

            for (int i = 0; i < files.Length; i++)
            {
                Project prj = new Project();
                if (!prj.LoadFromXML(files[i]))
                {
                    items.Clear();
                    return false;
                }

                prj.Manager = manager;
                prj.DbId = DBModule.AddProject(prj.Name, prj.Id, prj.StartFrom);

                items.Add(prj);
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

        #endregion

        #region Public Methods

        public void Init()
        {
            if (isInitialized) return;
            projects.LoadProjects(this);
            isInitialized = true;

            Execute();
        }

        public bool Start()
        {
            Init();

            timer.Enabled = true;
            timer.Start();

            return true;
        }

        public void Resume()
        {
            timer.Enabled = true;
            timer.Start();
        }

        public void Stop()
        {
            timer.Enabled = false;
            timer.Stop();
        }

        public bool Execute()
        {
            Stop();

            for (int i = 0; i < projects.Items.Count; i++)
            {
                SyncManager.CurExecProject = (projects.Items[i] as Project);
                (projects.Items[i] as Project).Execute();
            }

            Resume();

            return true;
        }

        #endregion

        #region Private Members

        private void timer_Elapsed(object sender, EventArgs e)
        {
            Execute();
        }

        #endregion

        public SyncManager()
        {
            timer.Elapsed += new System.Timers.ElapsedEventHandler(timer_Elapsed);
            timer.Enabled = false;
            timer.Interval = 3600000;
            //timer.Interval = 5000;
        }
    }
}
