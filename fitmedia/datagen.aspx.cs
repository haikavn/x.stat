using System;
using System.Collections.Generic;
using System.Data;
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
            APIServiceProviderNamespace.main.projectsDataTable dt = DBModule.GetProjects();

            if (dt == null)
            {
                Response.Redirect("http://xstat.fitmedia.ru");
                return;
            }

            string path = Request.PhysicalApplicationPath;

/*            string treehtml = "<ul>";
            string treejs = "var items = 0;var n = 0;var it;var root;\r\n\r\n";

            foreach (APIServiceProviderNamespace.main.projectsRow pr in dt.Rows)
            {
                treehtml += "<li class=\"p" + pr.id + "\" >" + pr.name + "<ul>";

                treejs += "$('#jqxTree').jqxTree('addTo', { label: '" + pr.name + "' });\r\n";
                treejs += "items = $('#jqxTree').jqxTree('getItems');\r\n";
                treejs += "root = items[n];\r\n";
                treejs += "root.element.name = \"" + pr.id.ToString() + "\";\r\n";
                treejs += "$('#jqxTree').jqxTree('checkItem', root.element, true);";
                treejs += "n++;\r\n\r\n";


                APIServiceProviderNamespace.main.campaignsDataTable cdt = DBModule.GetCampaignsByProjectID(pr.id);

                foreach (APIServiceProviderNamespace.main.campaignsRow cr in cdt.Rows)
                {
                    treehtml += "<li class=\"p" + cr.id + "\" >" + cr.name + "</li>";
                    treejs += "$('#jqxTree').jqxTree('addTo', { label: '" + cr.name + "' }, root.element);\r\n";
                    treejs += "items = $('#jqxTree').jqxTree('getItems');\r\n";
                    treejs += "it = items[n];\r\n";
                    treejs += "it.element.name = \"" + cr.id.ToString() + "\";\r\n";
                    treejs += "$('#jqxTree').jqxTree('checkItem', it.element, true);";
                    treejs += "n++;\r\n\r\n";
                }


                treehtml += "</ul></li>";
            }

            treehtml += "</ul>";

            System.IO.File.WriteAllText(path + "\\ui\\tree.html", treehtml);
            System.IO.File.WriteAllText(path + "\\ui\\tree.js", treejs + "isfirstload = false;");*/

            string xmldata = "<?xml version='1.0' encoding='utf-8'?>\n";
            xmldata += "<rows>\n";

            DateTime startdate = DateTime.Now;
            DateTime enddate = DateTime.Now;

            int groupby = 2;

            if (Request.Params["g"] != null)
                int.TryParse(Request.Params["g"].ToString(), out groupby);

            if (Request.Params["sd"] != null)
            {
                try
                {
                    startdate = DateTime.ParseExact(Request.Params["sd"].ToString(), "dd.MM.yyyy", null);
                }
                catch
                {
                }
            }
            else
                startdate = startdate.AddDays(-(7 * 20));

            if (Request.Params["ed"] != null)
            {
                try
                {
                    enddate = DateTime.ParseExact(Request.Params["ed"].ToString(), "dd.MM.yyyy", null);
                }
                catch
                {
                }
            }

            /*Hashtable projectids = new Hashtable();
            Hashtable campaignids = new Hashtable();

            if (Request.Params["pids"] != null)
            {
                string[] str = Request.Params["pids"].Split(new char[1] {','});
                int n = 0;
                for (int i = 0; i < str.Length; i++)
                    if (int.TryParse(str[i], out n))
                        projectids[n] = n;
            }

            if (Request.Params["cids"] != null)
            {
                string[] str = Request.Params["cids"].Split(new char[1] { ',' });
                int n = 0;
                for (int i = 0; i < str.Length; i++)
                    if (int.TryParse(str[i], out n))
                        campaignids[n] = n;
            }*/


            int pid = 0;

            if (Request.Params["pid"] != null)
                int.TryParse(Request.Params["pid"].ToString(), out pid);

            if (groupby == 2)
            {
                while (startdate.DayOfWeek != DayOfWeek.Monday && startdate.Month >= 1 && startdate.Day > 1)
                    startdate = startdate.AddDays(-1);
                while (enddate.DayOfWeek != DayOfWeek.Sunday)
                    enddate = enddate.AddDays(1);
            }
            else
                if (groupby == 3)
                {
                    int m = startdate.Month;
                    while (startdate.Month == m)
                        startdate = startdate.AddDays(-1);
                    startdate = startdate.AddDays(1);
                    m = enddate.Month;
                    while (enddate.Month == m)
                        enddate = enddate.AddDays(1);
                    enddate = enddate.AddDays(-1);
                }

            DateTime st = startdate;
            DateTime ed = enddate;

            string linksjs = "$('#startdate').val('" + st.ToString("dd.MM.yyyy") + "');$('#enddate').val('" + ed.ToString("dd.MM.yyyy") + "');$('<option />').attr('value', '0').text('All').appendTo('#projects');";


            string sel1 = ".attr('selected', 'selected')";
            string sel2 = ".attr('selected', 'selected')";
            string sel3 = ".attr('selected', 'selected')";

            if (groupby == 1)
            {
                sel2 = "";
                sel3 = "";
            }
            else
                if (groupby == 2)
                {
                    sel1 = "";
                    sel3 = "";
                }
                else
                    if (groupby == 3)
                    {
                        sel1 = "";
                        sel2 = "";
                    }

            linksjs += "$('<option />').attr('value', '1')" + sel1 + ".text('По дням').appendTo('#groups');";
            linksjs += "$('<option />').attr('value', '2')" + sel2 + ".text('По неделям').appendTo('#groups');";
            linksjs += "$('<option />').attr('value', '3')" + sel3 + ".text('По месяцам').appendTo('#groups');";

            string chartdata1 = "";
            string griddata = "";
            int catcount1 = 0;
            string cat1 = "";
            string totalsums = "";
            string totalsumas = "";
            string totalsumrs = "";
            string totalsumds = "";
            string totalsumdds = "";
            string expensess = "";

            string json = "[";

            bool bl = true;

            int records = 0;

            while (startdate < enddate || (startdate <= enddate && groupby == 1))
            {
                records++;

                startdate = new DateTime(startdate.Year, startdate.Month, startdate.Day, 0, 0, 0);
                                      
                ed = startdate;
                if (groupby == 2)
                {
                    while (ed.DayOfWeek != DayOfWeek.Sunday)
                        ed = ed.AddDays(1);
                }
                else
                    if (groupby == 3)
                    {
                        ed = ed.AddMonths(1);
                        ed = ed.AddDays(-1);
                    }


                ed = new DateTime(ed.Year, ed.Month, ed.Day, 23, 59, 59);

                int total = 0;
                int totala = 0;
                int totalr = 0;
                decimal totalsum = 0M;
                decimal totalsuma = 0M;
                decimal totalsumr = 0M;
                decimal totalsumd = 0M;
                decimal totalsumdd = 0M;
                int clicks = 0;
                int shows = 0;
                decimal expenses = 0M;

                json += "{";

                foreach (APIServiceProviderNamespace.main.projectsRow pr in dt.Rows)
                {
                    if (bl)
                    {
                        if (pid == 0 || (pid > 0 && pid != pr.id))
                            linksjs += "$('<option />').attr('value', '" + pr.id.ToString() + "').text('" + pr.name + "').appendTo('#projects');";
                        else
                        {
                            // if (pid == pr.id)
                            linksjs += "$('<option />').attr('value', '" + pr.id.ToString() + "').attr('selected', 'selected').text('" + pr.name + "').appendTo('#projects');";
                        }

                    }


                    if (pid > 0 && pr.id != pid) continue;
                    //linksjs += "$('<a />').attr('href', 'datagen.aspx?pid=" + pr.id.ToString() + "').text(\"" + pr.name + "\").appendTo('#links');";
                    //linksjs += "$('#projects').append($('<option></option>').val(" + pr.id.ToString() + ").html(" + pr.name + "));";

               //     if (projectids[pr.id] == null)
                      //  continue;

                    APIServiceProviderNamespace.main.ordersTotalsDataTable dt1 = DBModule.GetOrdersTotals(startdate, ed, pr.id);
                    //APIServiceProviderNamespace.main.ordersTotalsDataTable dt1 = DBModule.GetOrdersTotals(pr.startfrom, DateTime.Now, pr.id);

                    if (dt1.Rows.Count > 0)
                    {
                        APIServiceProviderNamespace.main.ordersTotalsRow or = dt1.Rows[0] as APIServiceProviderNamespace.main.ordersTotalsRow;
                        total += or.total;
                        totala += or.totala;
                        totalr += or.totalr;
                        totalsum += or.totalsum;
                        totalsuma += or.totalsuma;
                        totalsumr += or.totalsumr;
                        totalsumd += or.totalsumd;
                        totalsumdd += or.totalsumdd;
                    }

                    APIServiceProviderNamespace.main.campaignsDataTable cdt = DBModule.GetCampaignsByProjectID(pr.id);

                    foreach (APIServiceProviderNamespace.main.campaignsRow cr in cdt.Rows)
                    {
                        //if (campaignids[cr.id] == null)
                          //  continue;

                        APIServiceProviderNamespace.main.statTotalsDataTable dt2 = DBModule.GetStatTotals(startdate, ed, pr.id, cr.id);

                        if (dt2.Rows.Count > 0)
                        {
                            clicks += Convert.ToInt32(dt2.Rows[0]["clicks"]);
                            shows += Convert.ToInt32(dt2.Rows[0]["shows"]);
                            expenses += Convert.ToDecimal(dt2.Rows[0]["expenses"]);
                        }
                    }
                }

                json += "\"period\":\"" + startdate.ToString("dd.MM.yyyy") + " - " + ed.ToString("dd.MM.yyyy") + "\",";
                json += "\"total\": \"" + total.ToString() + "\",";
                json += "\"totala\": \"" + totala.ToString() + "\",";
                json += "\"totalr\": \"" + totalr.ToString() + "\",";
                json += "\"totalsum\": \"" + (totalsum).ToString().Replace(",", ".") + "\",";
                json += "\"totalsuma\": \"" + (totalsuma).ToString().Replace(",", ".") + "\",";
                json += "\"totalsumr\": \"" + (totalsumr).ToString().Replace(",", ".") + "\",";
                json += "\"totalsumd\": \"" + (totalsumd).ToString().Replace(",", ".") + "\",";
                json += "\"totalsumdd\": \"" + (totalsumdd).ToString().Replace(",", ".") + "\",";
                json += "\"clicks\": \"" + clicks.ToString() + "\",";
                json += "\"shows\": \"" + shows.ToString() + "\",";
                json += "\"expenses\": \"" + (expenses).ToString().Replace(",", ".") + "\"";

                griddata += "row = {};";
                griddata += "row[\"period\"] = \"" + startdate.ToString("dd.MM.yyyy") + " - " + ed.ToString("dd.MM.yyyy") + "\";";
                griddata += "row[\"total\"] = \"" + total.ToString() + "\";";
                griddata += "row[\"totala\"] = \"" + totala.ToString() + "\";";
                griddata += "row[\"totalr\"] = \"" + totalr.ToString() + "\";";
                griddata += "row[\"totalsum\"] = \"" + totalsum.ToString("### ### ###") + " р." + "\";";
                griddata += "row[\"totalsuma\"] = \"" + totalsuma.ToString("### ### ###") + " р." + "\";";
                griddata += "row[\"totalsumr\"] = \"" + totalsumr.ToString("### ### ###") + " р." + "\";";
                griddata += "row[\"totalsumd\"] = \"" + totalsumd.ToString("### ### ###") + " р." + "\";";
                griddata += "row[\"totalsumdd\"] = \"" + totalsumdd.ToString("### ### ###") + " р." + "\";";
                griddata += "row[\"clicks\"] = \"" + clicks.ToString() + "\";";
                griddata += "row[\"shows\"] = \"" + shows.ToString() + "\";";
                griddata += "row[\"expenses\"] = \"" + expenses.ToString("### ### ###") + " р." + "\";";
                griddata += "data[i]=row;";
                griddata += "i++;";


                json += "},";

                cat1 += "'" + startdate.ToString("dd.MM.yyyy") + "-" + ed.ToString("dd.MM.yyyy") + "'";
                cat1 += ",";

                xmldata += "<row>\n";
                xmldata += "<cell>" + startdate.ToString("dd.MM.yyyy") + " - " + ed.ToString("dd.MM.yyyy") + "</cell>\n";
                xmldata += "<cell>" + total.ToString() + "</cell>\n";
                xmldata += "<cell>" + totala.ToString() + "</cell>\n";
                xmldata += "<cell>" + totalr.ToString() + "</cell>\n";
                if (totalsum > 0)
                    xmldata += "<cell>" + totalsum.ToString("### ### ###") + " р." + "</cell>\n";
                else
                    xmldata += "<cell>0 р.</cell>\n";
                if (totalsuma > 0)
                    xmldata += "<cell>" + totalsuma.ToString("### ### ###") + " р." + "</cell>\n";
                else
                    xmldata += "<cell>0 р.</cell>\n";
                if (totalsumr > 0)
                    xmldata += "<cell>" + totalsumr.ToString("### ### ###") + " р." + "</cell>\n";
                else
                    xmldata += "<cell>0 р.</cell>\n";
                if (totalsumd > 0)
                    xmldata += "<cell>" + totalsumd.ToString("### ### ###") + " р." + "</cell>\n";
                else
                    xmldata += "<cell>0 р.</cell>\n";
                if (totalsumdd > 0)
                    xmldata += "<cell>" + totalsumdd.ToString("### ### ###") + " р." + "</cell>\n";
                else
                    xmldata += "<cell>0 р.</cell>\n";
                xmldata += "<cell>" + clicks.ToString() + "</cell>\n";
                xmldata += "<cell>" + shows.ToString() + "</cell>\n";
                if (expenses > 0)
                    xmldata += "<cell>" + expenses.ToString("### ### ###") + " р." + "</cell>\n";
                else
                    xmldata += "<cell>0 р.</cell>\n";
                xmldata += "</row>\n";

                totalsums += totalsum.ToString().Replace(",", ".") + ",";
                totalsumas += totalsuma.ToString().Replace(",", ".") + ",";
                totalsumrs += totalsumr.ToString().Replace(",", ".") + ",";
                totalsumds += totalsumd.ToString().Replace(",", ".") + ",";
                totalsumdds += totalsumdd.ToString().Replace(",", ".") + ",";
                expensess += expenses.ToString().Replace(",", ".") + ",";

                if (groupby == 2)
                {
                    int dow = (int)startdate.DayOfWeek;
                    if (dow == 0) dow = 7;
                    int days = 7 - dow + 1;
                    startdate = startdate.AddDays(days);
                }
                else
                    if (groupby == 1)
                        startdate = startdate.AddDays(1);
                    else
                        if (groupby == 3)
                            startdate = startdate.AddMonths(1);

                bl = false;
            }

            json = json.Remove(json.Length - 1);

            json += "]";

           // xmldata += "<total>" + ((int)(records / 10)).ToString() + "</total>";
           // xmldata += "<records>" + records.ToString() + "</records>";
            xmldata += "</rows>\n";

            cat1 = cat1.Remove(cat1.Length - 1);
            totalsums = totalsums.Remove(totalsums.Length - 1);
            totalsumas = totalsumas.Remove(totalsumas.Length - 1);
            totalsumrs = totalsumrs.Remove(totalsumrs.Length - 1);
            totalsumds = totalsumds.Remove(totalsumds.Length - 1);
            totalsumdds = totalsumdds.Remove(totalsumdds.Length - 1);
            expensess = expensess.Remove(expensess.Length - 1);

            chartdata1 += "{\r\n";
            chartdata1 += "name: 'Общ. сумма заказов',\r\n";
            chartdata1 += "data: [" + totalsums + "]\r\n";
            chartdata1 += "},\r\n";

            chartdata1 += "{\r\n";
            chartdata1 += "name: 'Сумма под. заказов',\r\n";
            chartdata1 += "data: [" + totalsumas + "]\r\n";
            chartdata1 += "},\r\n";

            chartdata1 += "{\r\n";
            chartdata1 += "name: 'Сумма отказов',\r\n";
            chartdata1 += "data: [" + totalsumrs + "]\r\n";
            chartdata1 += "},\r\n";

            chartdata1 += "{\r\n";
            chartdata1 += "name: 'Сумма скидок',\r\n";
            chartdata1 += "data: [" + totalsumdds + "]\r\n";
            chartdata1 += "},";

            chartdata1 += "{\r\n";
            chartdata1 += "name: 'Реал. сумма заказов',\r\n";
            chartdata1 += "data: [" + totalsumds + "]\r\n";
            chartdata1 += "},\r\n";

            chartdata1 += "{\r\n";
            chartdata1 += "name: 'Расходы',\r\n";
            chartdata1 += "data: [" + expensess + "]\r\n";
            chartdata1 += "}";



            linksjs += "$('#navigate').attr('href', 'datagen.aspx?sd=' + $('#startdate').val() + '&ed=' + $('#enddate').val() + '&pid=' + $('#projects').val() + '&g=' + $('#groups').val());";

            //linksjs += "jQuery('#datagrid').jqGrid({ url: 'datagen.aspx?sd=" + st.ToString("dd.MM.yyyy") + "&ed=" + ed.ToString("dd.MM.yyyy") + "&pid=" + pid.ToString() + "' });";

            string js = System.IO.File.ReadAllText(path + "\\ui\\ui_tpl.js");

            js = js.Replace("{[(CONT1)]}", "container").Replace("{[(TITLE1)]}", "График за период времени").Replace("{[(SUBTITLE1)]}", st.ToString("dd.MM.yyyy") + "-" + ed.ToString("dd.MM.yyyy")).Replace("{[(CAT1)]}", cat1).Replace("{[(DATA1)]}", chartdata1).Replace("{[(LINKS)]}", linksjs);
            //js = js.Replace("{[(GDATA)]}", griddata);

            System.IO.File.WriteAllText(path + "\\ui\\ui.js", js);
            System.IO.File.WriteAllText(path + "\\ui\\griddata.xml", xmldata);

            Response.ContentType = "text/xml";
            Response.Write(xmldata);
            Response.Redirect("http://xstat.fitmedia.ru");
            //Response.Redirect("index.html");
        }
    }
}
