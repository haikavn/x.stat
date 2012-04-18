using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using APIServiceProviderNamespace.mainTableAdapters;
using System.Data.SqlClient;
using System.Xml;
using System.IO;
using System.Reflection;
using System.IO.Compression;


namespace APIServiceProviderNamespace
{
    public class DBModule
    {
        public static string lastErrorString = "";
        public static SqlConnection sqlCon = null;

        public static void InitConnection()
        {
            if (sqlCon != null) return;
            
            string path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().CodeBase);
            XmlDocument xmldoc = new XmlDocument();
            xmldoc.Load(path.Replace("file:\\", "") + "\\db.xml");

            XmlElement root = xmldoc["ConnectionStrings"];
            if (root == null) return;

            XmlElement cs = root["ConnectionString"];
            if (cs == null) return;

            sqlCon = new SqlConnection(cs.InnerText);
        }

        public static bool AddStatisticsRecord(int campaignid, DateTime datetime, int views, int clicks, decimal price, string data, StatLevel sl)
        {
            try
            {
                InitConnection();

                statisticsTableAdapter adapter = new statisticsTableAdapter();
                if (adapter.Connection != null)
                    adapter.Connection.Close();

                adapter.Connection = sqlCon;
                adapter.sp_AddStatisticsRecord(campaignid, datetime, views, clicks, price, data, (int)sl);
                return true;
            }
            catch (Exception exObj)
            {
                lastErrorString = exObj.Message;
                string path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().CodeBase);
                System.IO.File.AppendAllText(path.Replace("file:\\", "") + "\\dblog.txt", "[AddStatisticsRecord - " + DateTime.Now.ToString() + "]" + exObj.Message + "\r\n");
            }

            return false;
        }

        public static bool AddTrafficSource(DateTime dt, int projectid, string source, string medium, string term, int campaignid, string keyword, long impressions, long clicks, short ctr, decimal costperclick)
        {
            try
            {
                InitConnection();

                trafficsourcesTableAdapter adapter = new trafficsourcesTableAdapter();
                if (adapter.Connection != null)
                    adapter.Connection.Close();

                adapter.Connection = sqlCon;
                adapter.sp_AddTrafficSource(dt, projectid, source, medium, term, campaignid, keyword, impressions, clicks, ctr, costperclick);
                return true;
            }
            catch (Exception exObj)
            {
                lastErrorString = exObj.Message;
                string path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().CodeBase);
                System.IO.File.AppendAllText(path.Replace("file:\\", "") + "\\dblog.txt", "[AddTrafficSource - " + DateTime.Now.ToString() + "]" + exObj.Message + "\r\n");
            }

            return false;
        }

        public static int AddCompaign(string name, string cid, int projectid, string provider, string client)
        {
            try
            {
                InitConnection();

                campaignsTableAdapter adapter = new campaignsTableAdapter();
                if (adapter.Connection != null)
                    adapter.Connection.Close();

                adapter.Connection = sqlCon;
                return Convert.ToInt32(adapter.sp_AddCampaign(name, cid, projectid, provider, client));
            }
            catch (Exception exObj)
            {
                lastErrorString = exObj.Message;
                string path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().CodeBase);
                System.IO.File.AppendAllText(path.Replace("file:\\", "") + "\\dblog.txt", "[AddCampaign - " + DateTime.Now.ToString() + "]" + exObj.Message + "\r\n");
            }

            return 0;
        }

        public static APIServiceProviderNamespace.main.campaignsDataTable GetCampaignByCID(string cid)
        {
            try
            {
                InitConnection();

                campaignsTableAdapter adapter = new campaignsTableAdapter();
                if (adapter.Connection != null)
                    adapter.Connection.Close();
                adapter.Connection = sqlCon;
                return adapter.sp_GetCampaignByCID(cid);
            }
            catch (Exception exObj)
            {
                lastErrorString = exObj.Message;
                return null;
            }
        }

        public static APIServiceProviderNamespace.main.campaignsDataTable GetCampaignsByProjectID(int projectid)
        {
            try
            {
                InitConnection();

                campaignsTableAdapter adapter = new campaignsTableAdapter();
                if (adapter.Connection != null)
                    adapter.Connection.Close();
                adapter.Connection = sqlCon;
                return adapter.sp_GetCampaignsByProjectID(projectid);
            }
            catch (Exception exObj)
            {
                lastErrorString = exObj.Message;
                return null;
            }
        }

        
        public static int AddOrder(int oid, string status, DateTime dateordered, DateTime datetodeliver, decimal deliverycost, decimal deliveryprice, bool iscash, decimal additionaldeliverysum, decimal sum, decimal discounted, string source, int projectid)
        {
            try
            {
                InitConnection();

                ordersTableAdapter adapter = new ordersTableAdapter();
                if (adapter.Connection != null)
                    adapter.Connection.Close();

                adapter.Connection = sqlCon;

                return Convert.ToInt32(adapter.sp_AddOrder(oid, status, dateordered, datetodeliver, deliverycost, deliveryprice, iscash, additionaldeliverysum, sum, discounted, source, projectid));
            }
            catch (Exception exObj)
            {
                lastErrorString = exObj.Message;
                string path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().CodeBase);
                System.IO.File.AppendAllText(path.Replace("file:\\", "") + "\\dblog.txt", "[AddOrder - " + DateTime.Now.ToString() + "]" + exObj.Message + "\r\n");
            }

            return 0;
        }

        public static bool AddOrderDetails(int orderid, int productid, string producttitle, string productstatus, float quantity, decimal price, decimal totalsum, decimal discountprice, bool isreturned)
        {
            try
            {
                InitConnection();

                orderdetailsTableAdapter adapter = new orderdetailsTableAdapter();
                if (adapter.Connection != null)
                    adapter.Connection.Close();

                adapter.Connection = sqlCon;
                adapter.Insert(orderid, productid, producttitle, productstatus, quantity, price, totalsum, discountprice, isreturned);

                return true;
            }
            catch (Exception exObj)
            {
                lastErrorString = exObj.Message;
                string path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().CodeBase);
                System.IO.File.AppendAllText(path.Replace("file:\\", "") + "\\dblog.txt", "[AddOrderDetails - " + DateTime.Now.ToString() + "]" + exObj.Message + "\r\n");
            }

            return false;
        }

        public static APIServiceProviderNamespace.main.ordersDataTable GetOrders()
        {
            try
            {
                InitConnection();

                ordersTableAdapter adapter = new ordersTableAdapter();
                if (adapter.Connection != null)
                    adapter.Connection.Close();

                adapter.Connection = sqlCon;

                return adapter.GetData();
            }
            catch (Exception exObj)
            {
                lastErrorString = exObj.Message;
                return null;
            }
        }

        public static APIServiceProviderNamespace.main.WeeklyOrdersCountDataTable GetWeeklyOrdersCount(int year)
        {
            try
            {
                InitConnection();

                WeeklyOrdersCountTableAdapter adapter = new WeeklyOrdersCountTableAdapter();
                if (adapter.Connection != null)
                    adapter.Connection.Close();

                adapter.Connection = sqlCon;

                return adapter.GetData(year);
            }
            catch (Exception exObj)
            {
                lastErrorString = exObj.Message;
                return null;
            }
        }

        public static int AddProject(string name, string pid, DateTime startfrom)
        {
            try
            {
                InitConnection();

                projectsTableAdapter adapter = new projectsTableAdapter();
                if (adapter.Connection != null)
                    adapter.Connection.Close();

                adapter.Connection = sqlCon;
                return Convert.ToInt32(adapter.sp_AddProject(name, pid, startfrom));
            }
            catch (Exception exObj)
            {
                lastErrorString = exObj.Message;
                string path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().CodeBase);
                System.IO.File.AppendAllText(path.Replace("file:\\", "") + "\\dblog.txt", "[AddProject - " + DateTime.Now.ToString() + "]" + exObj.Message + "\r\n");
            }

            return 0;
        }

        public static APIServiceProviderNamespace.main.projectsDataTable GetProjects()
        {
            try
            {
                InitConnection();
                projectsTableAdapter adapter = new projectsTableAdapter();
                if (adapter.Connection != null)
                    adapter.Connection.Close();

                adapter.Connection = sqlCon;

                return adapter.GetData();
            }
            catch (Exception exObj)
            {
                lastErrorString = exObj.Message;
                string path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().CodeBase);
                System.IO.File.AppendAllText(path.Replace("file:\\", "") + "\\dblog.txt", "[GetProjects - " + DateTime.Now.ToString() + "]" + exObj.Message + "\r\n");

                return null;
            }
        }

        public static bool AddKeyword(int campaignid, string keywordid, string keyword)
        {
            try
            {
                InitConnection();

                keywordsTableAdapter adapter = new keywordsTableAdapter();
                if (adapter.Connection != null)
                    adapter.Connection.Close();

                adapter.Connection = sqlCon;
                adapter.sp_AddKeyword(campaignid, keywordid, keyword);

                return true;
            }
            catch (Exception exObj)
            {
                lastErrorString = exObj.Message;
                string path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().CodeBase);
                System.IO.File.AppendAllText(path.Replace("file:\\", "") + "\\dblog.txt", "[AddKeyword - " + DateTime.Now.ToString() + "]" + exObj.Message + "]\r\n");
            }

            return false;
        }

        public static APIServiceProviderNamespace.main.ordersTotalsDataTable GetOrdersTotals(DateTime startdate, DateTime enddate, int projectid)
        {
            try
            {
                InitConnection();

                ordersTotalsTableAdapter adapter = new ordersTotalsTableAdapter();
                if (adapter.Connection != null)
                    adapter.Connection.Close();

                adapter.Connection = sqlCon;
                return adapter.GetData(startdate, enddate, projectid);
            }
            catch (Exception exObj)
            {
                lastErrorString = exObj.Message;
                return null;
            }
        }
        

        public static APIServiceProviderNamespace.main.statTotalsDataTable GetStatTotals(DateTime startdate, DateTime enddate, int projectid, int campaignid)
        {
            try
            {
                InitConnection();

                statTotalsTableAdapter adapter = new statTotalsTableAdapter();
                if (adapter.Connection != null)
                    adapter.Connection.Close();

                adapter.Connection = sqlCon;
                return adapter.GetData(startdate, enddate, projectid, campaignid);
            }
            catch (Exception exObj)
            {
                lastErrorString = exObj.Message;
                return null;
            }
        }
    }
}
