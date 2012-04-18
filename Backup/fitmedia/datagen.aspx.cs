using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using APIServiceProviderNamespace;
using System.Collections;
using System.IO;
using System.Reflection;

namespace fitmedia
{
    public partial class datagen : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string p = "";
            if (Request.Params["pids"] != null)
                p = Request.Params["pids"].ToString();

  
            string[] pids = p.ToString().Split(new char[1] { ',' });

            Hashtable projectids = new Hashtable();

            int res = 0;
            for (int i = 0; i < pids.Length; i++)
                if (int.TryParse(pids[i], out res))
                    projectids[Convert.ToInt32(pids[i])] = Convert.ToInt32(pids[i]);


            if (Request.Params["cids"] != null)
            {
                p = Request.Params["cids"].ToString();
            }

            string[] cids = p.ToString().Split(new char[1] { ',' });

            Hashtable campaignids = new Hashtable();

            res = 0;
            for (int i = 0; i < cids.Length; i++)
                if (int.TryParse(cids[i], out res))
                    campaignids[Convert.ToInt32(cids[i])] = Convert.ToInt32(cids[i]);

            APIServiceProviderNamespace.main.projectsDataTable dt = DBModule.GetProjects();

            if (dt == null) return;

            DateTime startdate = new DateTime(2012, 1, 1);//DateTime.Now;
            DateTime enddate = DateTime.Now;

            if (Request.Params["startdate"] != null)
            {
                startdate = DateTime.ParseExact(Request.Params["startdate"].ToString(), "dd.MM.yyyy", null);
            }

            if (Request.Params["enddate"] != null)
            {
                enddate = DateTime.ParseExact(Request.Params["enddate"].ToString(), "dd.MM.yyyy", null);
            }

            string json = "[";

            while (startdate.DayOfWeek != DayOfWeek.Monday)
                startdate = startdate.AddDays(-1);
            while (enddate.DayOfWeek != DayOfWeek.Sunday)
                enddate = enddate.AddDays(1);

            while (startdate < enddate)
            {
                int total = 0;
                int totala = 0;
                int totalr = 0;
                decimal totalsum = 0;
                decimal totalsuma = 0;
                decimal totalsumr = 0;
                decimal totalsumd = 0;
                decimal totalsumdd = 0;
                int clicks = 0;
                int shows = 0;
                decimal expenses = 0;

                json += "{";

                foreach (APIServiceProviderNamespace.main.projectsRow pr in dt.Rows)
                {
                    if (projectids.Count > 0 && projectids[pr.id] == null)
                        continue;

                    APIServiceProviderNamespace.main.ordersTotalsDataTable dt1 = DBModule.GetOrdersTotals(startdate, startdate.AddDays(7), pr.id); 

                    if (dt1.Rows.Count > 0)
                    {
                        total += Convert.ToInt32(dt1.Rows[0]["total"]);
                        totala += Convert.ToInt32(dt1.Rows[0]["totala"]);
                        totalr += Convert.ToInt32(dt1.Rows[0]["totalr"]);
                        totalsum += Convert.ToDecimal(dt1.Rows[0]["totalsum"]);
                        totalsuma += Convert.ToDecimal(dt1.Rows[0]["totalsuma"]);
                        totalsumr += Convert.ToDecimal(dt1.Rows[0]["totalsumr"]);
                        totalsumd += Convert.ToDecimal(dt1.Rows[0]["totalsumd"]);
                        totalsumdd += Convert.ToDecimal(dt1.Rows[0]["totalsumdd"]);
                    }

                    APIServiceProviderNamespace.main.campaignsDataTable cdt = DBModule.GetCampaignsByProjectID(pr.id);

                    foreach (APIServiceProviderNamespace.main.campaignsRow cr in cdt.Rows)
                    {
                        if (campaignids.Count > 0 && campaignids[cr.id] == null)
                            continue;

                        APIServiceProviderNamespace.main.statTotalsDataTable dt2 = DBModule.GetStatTotals(startdate, startdate.AddDays(7), pr.id, cr.id);

                        if (dt2.Rows.Count > 0)
                        {
                            clicks += Convert.ToInt32(dt2.Rows[0]["clicks"]);
                            shows += Convert.ToInt32(dt2.Rows[0]["shows"]);
                            expenses += Convert.ToDecimal(dt2.Rows[0]["expenses"]);
                        }
                    }
                }

                json += "\"period\":\"" + startdate.ToShortDateString() + " - " + startdate.AddDays(7).ToShortDateString() + "\",";
                json += "\"total\": \"" + total.ToString() + "\",";
                json += "\"totala\": \"" + totala.ToString() + "\",";
                json += "\"totalr\": \"" + totalr.ToString() + "\",";
                json += "\"totalsum\": \"" + totalsum.ToString().Replace(",", ".") + "\",";
                json += "\"totalsuma\": \"" + totalsuma.ToString().Replace(",", ".") + "\",";
                json += "\"totalsumr\": \"" + totalsumr.ToString().Replace(",", ".") + "\",";
                json += "\"totalsumd\": \"" + totalsumd.ToString().Replace(",", ".") + "\",";
                json += "\"totalsumdd\": \"" + totalsumdd.ToString().Replace(",", ".") + "\",";
                json += "\"clicks\": \"" + clicks.ToString() + "\",";
                json += "\"shows\": \"" + shows.ToString() + "\",";
                json += "\"expenses\": \"" + expenses.ToString().Replace(",", ".") + "\"";

                json += "},";

                startdate = startdate.AddDays(7);
            }

            json = json.Remove(json.Length - 1);

            json += "]";


            /*System.IO.File.WriteAllText(Request.PhysicalApplicationPath + "ui\\data\\data.txt", json);

            string ui = System.IO.File.ReadAllText(Request.PhysicalApplicationPath + "\\ui\\ui_tpl.js");

            string treejs = "var treeItems, rootItem, rootItemElement;\r\n";

            int j = 0;

            foreach (APIServiceProviderNamespace.main.projectsRow pr in dt.Rows)
            {
                treejs += "$('#jqxTree').jqxTree('addTo', { label: '" + pr.name + "' });\r\n";
                treejs += "treeItems = $(\"#jqxTree\").jqxTree('getItems');\r\n";
                treejs += "rootItem = treeItems[" + j.ToString() + "];\r\n";
                treejs += "rootItemElement = rootItem.element;\r\n";

                APIServiceProviderNamespace.main.campaignsDataTable cdt = DBModule.GetCampaignsByProjectID(pr.id);

                foreach (APIServiceProviderNamespace.main.campaignsRow cr in cdt.Rows)
                {
                    treejs += "$('#jqxTree').jqxTree('addTo', { label: '" + cr.name + "' }, rootItemElement);\r\n\r\n";
                    j++;
                }

                j++;
            }

            ui = ui.Replace("{[(jqxTree)]}", treejs);

            System.IO.File.WriteAllText(Request.PhysicalApplicationPath + "ui\\ui.js", ui);*/

            //Response.Redirect("/index.html");

            Response.ContentType = "application/json"; 
            Response.Write(json);

            /* DateTime start = DateTime.Now;

                 while (start.DayOfWeek != DayOfWeek.Monday)
                     start = start.AddDays(-1);

                 while (start.Year <= DateTime.Now.Year)
                 {
                     APIServiceProviderNamespace.main.ordersTotalsDataTable dt1 = DBModule.GetOrdersTotals(start, start.AddDays(7), r.id);
                     APIServiceProviderNamespace.main.statTotalsDataTable dt2 = DBModule.GetStatTotals(start, start.AddDays(6), r.id);

                     DataRow row = dt.NewRow();

                     row["project"] = r.name;

                     int total = 0;
                     int totala = 0;
                     int totalr = 0;
                     decimal totalsum = 0;
                     decimal totalsuma = 0;
                     decimal totalsumr = 0;
                     decimal totalsumd = 0;
                     decimal totalsumdd = 0;
                     int clicks = 0;
                     int shows = 0;
                     decimal expenses = 0;

                     row["period"] = start.ToShortDateString() + " - " + start.AddDays(6).ToShortDateString();

                     if (dt1.Rows.Count > 0)
                     {
                         total = Convert.ToInt32(dt1.Rows[0]["total"]);
                         totala = Convert.ToInt32(dt1.Rows[0]["totala"]);
                         totalr = Convert.ToInt32(dt1.Rows[0]["totalr"]);
                         totalsum = Convert.ToDecimal(dt1.Rows[0]["totalsum"]);
                         totalsuma = Convert.ToDecimal(dt1.Rows[0]["totalsuma"]);
                         totalsumr = Convert.ToDecimal(dt1.Rows[0]["totalsumr"]);
                         totalsumd = Convert.ToDecimal(dt1.Rows[0]["totalsumd"]);
                         totalsumdd = Convert.ToDecimal(dt1.Rows[0]["totalsumdd"]);
                     }

                     if (dt2.Rows.Count > 0)
                     {
                         clicks = Convert.ToInt32(dt2.Rows[0]["clicks"]);
                         shows = Convert.ToInt32(dt2.Rows[0]["shows"]);
                         expenses = Convert.ToDecimal(dt2.Rows[0]["expenses"]);
                     }

         }*/
        }
    }
}
