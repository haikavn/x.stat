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

        public static bool AddStatisticsRecord(int campaignid, DateTime datetime, int views, int clicks, decimal price, string data, StatLevel sl, int projectid, string keyword)
        {
            try
            {
                InitConnection();

                statisticsTableAdapter adapter = new statisticsTableAdapter();
                if (adapter.Connection != null)
                    adapter.Connection.Close();

                adapter.Connection = sqlCon;
                adapter.sp_AddStatisticsRecord(campaignid, datetime, views, clicks, price, data, (int)sl, projectid, keyword);
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

        public static bool DeleteStatisticsRecords(int projectid, DateTime dt)
        {
            try
            {
                InitConnection();

                statisticsTableAdapter adapter = new statisticsTableAdapter();
                if (adapter.Connection != null)
                    adapter.Connection.Close();

                adapter.Connection = sqlCon;
                adapter.sp_DeleteStatisticsRecords(projectid, dt);
                return true;
            }
            catch (Exception exObj)
            {
                lastErrorString = exObj.Message;
                string path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().CodeBase);
                System.IO.File.AppendAllText(path.Replace("file:\\", "") + "\\dblog.txt", "[DeleteStatisticsRecords - " + DateTime.Now.ToString() + "]" + exObj.Message + "\r\n");
            }

            return false;
        }

        public static APIServiceProviderNamespace.main.GetCustomExpensesDataTable GetCustomExpenses(DateTime startdate, DateTime enddate, int projectid)
        {
            try
            {
                InitConnection();

                GetCustomExpensesTableAdapter adapter = new GetCustomExpensesTableAdapter();
                if (adapter.Connection != null)
                    adapter.Connection.Close();

                adapter.Connection = sqlCon;
                return adapter.GetData(startdate, enddate, projectid);
            }
            catch (Exception exObj)
            {
                lastErrorString = exObj.Message;
                string path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().CodeBase);
                System.IO.File.AppendAllText(path.Replace("file:\\", "") + "\\dblog.txt", "[GetStatisticsRecords - " + DateTime.Now.ToString() + "]" + exObj.Message + "\r\n");
            }

            return null;
        }

        public static int UpdateCustomExpenses(int id, int projectid, int campaignid, DateTime dt, int clicks, int shows, decimal price, decimal expenses)
        {
            try
            {
                InitConnection();

                customexpensesTableAdapter adapter = new customexpensesTableAdapter();
                if (adapter.Connection != null)
                    adapter.Connection.Close();

                adapter.Connection = sqlCon;
                return Convert.ToInt32(adapter.sp_UpdateCustomExpenses(id, projectid, campaignid, dt, clicks, shows, price, expenses));
            }
            catch (Exception exObj)
            {
                lastErrorString = exObj.Message;
                string path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().CodeBase);
                System.IO.File.AppendAllText(path.Replace("file:\\", "") + "\\dblog.txt", "[UpdateCustomExpenses - " + DateTime.Now.ToString() + "]" + exObj.Message + "\r\n");
            }

            return 0;
        }



        public static int AddCompaign(string name, string cid, int projectid, string provider, string client, APIServiceProviderTypes ptype, decimal balance, decimal maxtransfer)
        {
            try
            {
                InitConnection();

                campaignsTableAdapter adapter = new campaignsTableAdapter();
                if (adapter.Connection != null)
                    adapter.Connection.Close();

                adapter.Connection = sqlCon;
                return Convert.ToInt32(adapter.sp_AddCampaign(name, cid, projectid, provider, client, (int)ptype, balance, maxtransfer));
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

        
        public static int AddOrder(int oid, string status, DateTime dateordered, DateTime datetodeliver, decimal deliverycost, decimal deliveryprice, bool iscash, decimal additionaldeliverysum, decimal sum, decimal discounted, string source, int projectid, string ordersource, string deliverycity)
        {
            try
            {
                InitConnection();

                ordersTableAdapter adapter = new ordersTableAdapter();
                if (adapter.Connection != null)
                    adapter.Connection.Close();

                adapter.Connection = sqlCon;

                return Convert.ToInt32(adapter.sp_AddOrder(oid, status, dateordered, datetodeliver, deliverycost, deliveryprice, iscash, additionaldeliverysum, sum, discounted, source, projectid, ordersource, deliverycity));
            }
            catch (Exception exObj)
            {
                lastErrorString = exObj.Message;
                string path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().CodeBase);
                System.IO.File.AppendAllText(path.Replace("file:\\", "") + "\\dblog.txt", "[AddOrder - " + DateTime.Now.ToString() + "]" + exObj.Message + "\r\n");
            }

            return 0;
        }

        public static bool DeleteOrders(int projectid, DateTime dt)
        {
            try
            {
                InitConnection();

                ordersTableAdapter adapter = new ordersTableAdapter();
                if (adapter.Connection != null)
                    adapter.Connection.Close();

                adapter.Connection = sqlCon;

                adapter.sp_DeleteOrders(projectid, dt);
                return true;
            }
            catch (Exception exObj)
            {
                lastErrorString = exObj.Message;
                string path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().CodeBase);
                System.IO.File.AppendAllText(path.Replace("file:\\", "") + "\\dblog.txt", "[DeleteOrders - " + DateTime.Now.ToString() + "]" + exObj.Message + "\r\n");
            }

            return false;
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

        public static APIServiceProviderNamespace.main.ordersDataTable GetNotDeliveredOrders()
        {
            try
            {
                InitConnection();

                ordersTableAdapter adapter = new ordersTableAdapter();
                if (adapter.Connection != null)
                    adapter.Connection.Close();

                adapter.Connection = sqlCon;

                return adapter.sp_GetNotDeliveredOrders();
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

        public static APIServiceProviderNamespace.main.ordersTotalsDataTable GetOrdersTotals(DateTime startdate, DateTime enddate, int projectid, int city, int source, int payment, int days)
        {
            try
            {
                InitConnection();

                ordersTotalsTableAdapter adapter = new ordersTotalsTableAdapter();
                if (adapter.Connection != null)
                    adapter.Connection.Close();

                adapter.Connection = sqlCon;
                return adapter.GetData(startdate, enddate, projectid, city, source, payment, days);
            }
            catch (Exception exObj)
            {
                lastErrorString = exObj.Message;
                return null;
            }
        }
        

        public static APIServiceProviderNamespace.main.statTotalsDataTable GetStatTotals(DateTime startdate, DateTime enddate, int projectid, int campaignid, int days, int lg)
        {
            try
            {
                InitConnection();

                statTotalsTableAdapter adapter = new statTotalsTableAdapter();
                if (adapter.Connection != null)
                    adapter.Connection.Close();

                adapter.Connection = sqlCon;
                return adapter.GetData(startdate, enddate, projectid, campaignid, days, lg);
            }
            catch (Exception exObj)
            {
                lastErrorString = exObj.Message;
                return null;
            }
        }

        public static int AddLIReport(string name)
        {
            try
            {
                InitConnection();

                li_reportsTableAdapter adapter = new li_reportsTableAdapter();
                if (adapter.Connection != null)
                    adapter.Connection.Close();

                adapter.Connection = sqlCon;
                return Convert.ToInt32(adapter.sp_AddLIReport(name));
            }
            catch (Exception exObj)
            {
                lastErrorString = exObj.Message;
                return 0;
            }
        }

        public static bool AddLIDetail(int projectid, int reportid, DateTime dt, int quantity, string name)
        {
            try
            {
                InitConnection();

                li_detailsTableAdapter adapter = new li_detailsTableAdapter();
                if (adapter.Connection != null)
                    adapter.Connection.Close();

                adapter.Connection = sqlCon;
                adapter.Insert(reportid, dt, quantity, name, projectid);
                return true;
            }
            catch (Exception exObj)
            {
                lastErrorString = exObj.Message;
                return false;
            }
        }

        public static APIServiceProviderNamespace.main.li_reportsDataTable GetLIReports()
        {
            try
            {
                InitConnection();

                li_reportsTableAdapter adapter = new li_reportsTableAdapter();
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

        public static void DeleteNotWorkingDays(int year)
        {
            try
            {
                InitConnection();

                notworkingdaysTableAdapter adapter = new notworkingdaysTableAdapter();
                if (adapter.Connection != null)
                    adapter.Connection.Close();

                adapter.Connection = sqlCon;
                adapter.sp_DeleteNotWorkingDays(year);
            }
            catch (Exception exObj)
            {
                lastErrorString = exObj.Message;
            }
        }

        public static void AddNotWorkingDay(DateTime dt)
        {
            try
            {
                InitConnection();

                notworkingdaysTableAdapter adapter = new notworkingdaysTableAdapter();
                if (adapter.Connection != null)
                    adapter.Connection.Close();

                adapter.Connection = sqlCon;
                adapter.Insert(dt);
            }
            catch (Exception exObj)
            {
                lastErrorString = exObj.Message;
            }
        }

        public static bool IsNotWorkingDay(DateTime dt)
        {
            try
            {
                InitConnection();

                notworkingdaysTableAdapter adapter = new notworkingdaysTableAdapter();
                if (adapter.Connection != null)
                    adapter.Connection.Close();

                adapter.Connection = sqlCon;
                int res = Convert.ToInt32(adapter.sp_IsNotWorkngDay(dt));
                if (res == 1)
                    return true;
                else
                    return false;
            }
            catch (Exception exObj)
            {
                lastErrorString = exObj.Message;
                return false;
            }
        }

        public static int AddCoef(int projectid, DateTime dt, float coef, APIServiceProviderTypes ptype)
        {
            try
            {
                InitConnection();

                coefsTableAdapter adapter = new coefsTableAdapter();
                if (adapter.Connection != null)
                    adapter.Connection.Close();

                adapter.Connection = sqlCon;
                return Convert.ToInt32(adapter.sp_AddCoef(projectid, dt, coef, (int)ptype));
            }
            catch (Exception exObj)
            {
                lastErrorString = exObj.Message;
                string path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().CodeBase);
                System.IO.File.AppendAllText(path.Replace("file:\\", "") + "\\dblog.txt", "[AddCoef - " + DateTime.Now.ToString() + "]" + exObj.Message + "\r\n");
            }

            return 0;
        }

        public static float GetLatestCoef(int projectid, DateTime dt, APIServiceProviderTypes ptype)
        {
            try
            {
                InitConnection();

                coefsTableAdapter adapter = new coefsTableAdapter();
                if (adapter.Connection != null)
                    adapter.Connection.Close();

                adapter.Connection = sqlCon;
                return (float)adapter.sp_GetLatestCoef(projectid, dt, (int)ptype);
            }
            catch (Exception exObj)
            {
                lastErrorString = exObj.Message;
                string path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().CodeBase);
                System.IO.File.AppendAllText(path.Replace("file:\\", "") + "\\dblog.txt", "[GetLatestCoef - " + DateTime.Now.ToString() + "]" + exObj.Message + "\r\n");
            }

            return 0;
        }

        public static bool RecalcStatCache()
        {
            try
            {
                InitConnection();

                statcacheTableAdapter adapter = new statcacheTableAdapter();
                if (adapter.Connection != null)
                    adapter.Connection.Close();

                adapter.Connection = sqlCon;
                adapter.sp_RecalcStatCache();

                return true;
            }
            catch (Exception exObj)
            {
                lastErrorString = exObj.Message;
                string path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().CodeBase);
                System.IO.File.AppendAllText(path.Replace("file:\\", "") + "\\dblog.txt", "[RecalcStatCache - " + DateTime.Now.ToString() + "]" + exObj.Message + "\r\n");
            }

            return false;
        }

        public static int AddExecLog(int projectid, DateTime dt, bool iserror, string errormsg, string methodname)
        {
            try
            {
                InitConnection();

                execlogTableAdapter adapter = new execlogTableAdapter();
                if (adapter.Connection != null)
                    adapter.Connection.Close();

                adapter.Connection = sqlCon;

                return Convert.ToInt32(adapter.sp_AddExecLog(projectid, methodname, dt, iserror, errormsg));
            }
            catch (Exception exObj)
            {
                lastErrorString = exObj.Message;
                string path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().CodeBase);
                System.IO.File.AppendAllText(path.Replace("file:\\", "") + "\\dblog.txt", "[AddExecLog - " + DateTime.Now.ToString() + "]" + exObj.Message + "\r\n");
            }

            return 0;
        }

        public static DateTime GetLastExecLogDate(int projectid)
        {
            try
            {
                InitConnection();

                execlogTableAdapter adapter = new execlogTableAdapter();
                if (adapter.Connection != null)
                    adapter.Connection.Close();

                adapter.Connection = sqlCon;

                object o = adapter.sp_GetLastExecLogDate(projectid);

                if (o == null)
                    return new DateTime(1977, 1, 1);

                return Convert.ToDateTime(o);
            }
            catch (Exception exObj)
            {
                lastErrorString = exObj.Message;
                string path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().CodeBase);
                System.IO.File.AppendAllText(path.Replace("file:\\", "") + "\\dblog.txt", "[GetLastExecLogDate - " + DateTime.Now.ToString() + "]" + exObj.Message + "\r\n");
            }

            return new DateTime(1977, 1, 1);
        }

        public static APIServiceProviderNamespace.main.execlogDataTable GetLastExecLog(int projectid)
        {
            try
            {
                InitConnection();

                execlogTableAdapter adapter = new execlogTableAdapter();
                if (adapter.Connection != null)
                    adapter.Connection.Close();

                adapter.Connection = sqlCon;
                return adapter.sp_GetLastExecLog(projectid);
            }
            catch (Exception exObj)
            {
                lastErrorString = exObj.Message;
                string path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().CodeBase);
                System.IO.File.AppendAllText(path.Replace("file:\\", "") + "\\dblog.txt", "[GetLastExecLog - " + DateTime.Now.ToString() + "]" + exObj.Message + "\r\n");
            }

            return null;
        }

        public static APIServiceProviderNamespace.main.execlogDataTable GetLastExecLogWithError(int projectid)
        {
            try
            {
                InitConnection();

                execlogTableAdapter adapter = new execlogTableAdapter();
                if (adapter.Connection != null)
                    adapter.Connection.Close();

                adapter.Connection = sqlCon;
                return adapter.sp_GetLastExecLogWithError(projectid);
            }
            catch (Exception exObj)
            {
                lastErrorString = exObj.Message;
                string path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().CodeBase);
                System.IO.File.AppendAllText(path.Replace("file:\\", "") + "\\dblog.txt", "[GetLastExecLogWithError - " + DateTime.Now.ToString() + "]" + exObj.Message + "\r\n");
            }

            return null;
        }

        public static APIServiceProviderNamespace.main.execlogDataTable GetErrorLogs(int projectid)
        {
            try
            {
                InitConnection();

                execlogTableAdapter adapter = new execlogTableAdapter();
                if (adapter.Connection != null)
                    adapter.Connection.Close();

                adapter.Connection = sqlCon;
                return adapter.sp_GetErrorLogs(projectid);
            }
            catch (Exception exObj)
            {
                lastErrorString = exObj.Message;
                string path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().CodeBase);
                System.IO.File.AppendAllText(path.Replace("file:\\", "") + "\\dblog.txt", "[GetErrorLogs - " + DateTime.Now.ToString() + "]" + exObj.Message + "\r\n");
            }

            return null;
        }

        public static bool DeleteExecLog(int projectid, DateTime dt, string methodname)
        {
            try
            {
                InitConnection();

                execlogTableAdapter adapter = new execlogTableAdapter();
                if (adapter.Connection != null)
                    adapter.Connection.Close();

                adapter.Connection = sqlCon;
                adapter.sp_DeleteExecLog(projectid, dt, methodname);
                return true;
            }
            catch (Exception exObj)
            {
                lastErrorString = exObj.Message;
                string path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().CodeBase);
                System.IO.File.AppendAllText(path.Replace("file:\\", "") + "\\dblog.txt", "[DeleteExecLog - " + DateTime.Now.ToString() + "]" + exObj.Message + "\r\n");
            }

            return false;
        }

        public static APIServiceProviderNamespace.main.balancehistoryDataTable GetBalanceHistory(int campaignid, int days)
        {
            try
            {
                InitConnection();

                balancehistoryTableAdapter adapter = new balancehistoryTableAdapter();
                if (adapter.Connection != null)
                    adapter.Connection.Close();

                adapter.Connection = sqlCon;
                return adapter.sp_GetBalanceHistory(campaignid, days);
            }
            catch (Exception exObj)
            {
                lastErrorString = exObj.Message;
                string path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().CodeBase);
                System.IO.File.AppendAllText(path.Replace("file:\\", "") + "\\dblog.txt", "[GetBalanceHistory - " + DateTime.Now.ToString() + "]" + exObj.Message + "\r\n");
            }

            return null;
        }
    }
}
