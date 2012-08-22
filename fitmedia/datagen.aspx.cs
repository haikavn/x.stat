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
using System.Globalization;

namespace fitmedia
{
    public partial class datagen : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            APIServiceProviderNamespace.main.projectsDataTable dt = DBModule.GetProjects();

            if (dt == null)
            {
                Response.Redirect("index.htm");
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

            string[] months = new string[12];
            months[0] = "Январь";
            months[1] = "Февраль";
            months[2] = "Март";
            months[3] = "Апрель";
            months[4] = "Май";
            months[5] = "Июнь";
            months[6] = "Июль";
            months[7] = "Август";
            months[8] = "Сентябрь";
            months[9] = "Октябрь";
            months[10] = "Ноябрь";
            months[11] = "Декабрь";


            DataTable dtChart = new DataTable("Chart");
            DataColumn dcCurrent = null;
            DataRow drCurrent = null;

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

            int city = 0, source = 0, payment = 0, days = 0;

            if (Request.Params["city"] != null)
            {
                int.TryParse(Request.Params["city"], out city);
            }
            if (Request.Params["source"] != null)
            {
                int.TryParse(Request.Params["source"], out source);
            }
            if (Request.Params["payment"] != null)
            {
                int.TryParse(Request.Params["payment"], out payment);
            }

            if (Request.Params["days"] != null)
            {
                int.TryParse(Request.Params["days"], out days);
            }

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

            switch (city)
            {
                case 0:
                    linksjs += "$('#city').val( 0 ).attr('selected',true);";
                    break;
                case 1:
                    linksjs += "$('#city').val( 1 ).attr('selected',true);";
                    break;
                case 2:
                    linksjs += "$('#city').val( 2 ).attr('selected',true);";
                    break;
            }

            switch (source)
            {
                case 0:
                    linksjs += "$('#source').val( 0 ).attr('selected',true);";
                    break;
                case 1:
                    linksjs += "$('#source').val( 1 ).attr('selected',true);";
                    break;
                case 2:
                    linksjs += "$('#source').val( 2 ).attr('selected',true);";
                    break;
            }

            switch (payment)
            {
                case 0:
                    linksjs += "$('#payment').val( 0 ).attr('selected',true);";
                    break;
                case 1:
                    linksjs += "$('#payment').val( 1 ).attr('selected',true);";
                    break;
                case 2:
                    linksjs += "$('#payment').val( 2 ).attr('selected',true);";
                    break;
            }

            switch (days)
            {
                case 0:
                    linksjs += "$('#days').val( 0 ).attr('selected',true);";
                    break;
                case 1:
                    linksjs += "$('#days').val( 1 ).attr('selected',true);";
                    break;
                case 2:
                    linksjs += "$('#days').val( 2 ).attr('selected',true);";
                    break;
            }


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

            string percents = "";

            string json = "[";

            bool bl = true;

            int records = 0;

            int total = 0;
            int totala = 0;
            int totalr = 0;
            decimal totalsum = 0M;
            decimal totalsuma = 0M;
            decimal totalsumr = 0M;
            decimal totalsumd = 0M;
            decimal totalsumdd = 0M;
            int clicks = 0;
            int customclicks = 0;
            int shows = 0;
            decimal expenses = 0M;
            decimal customexp = 0M;

            DateTime chDt = enddate;
            if (groupby == 1)
                chDt = enddate.AddDays(-60);
            else
                if (groupby == 2)
                    chDt = enddate.AddDays(-60 * 7);
                else
                    if (groupby == 3)
                    {
                        chDt = enddate.AddMonths(-60);
                        if (chDt.Day <= 15)
                        {
                            while (chDt.Day > 1) chDt = chDt.AddDays(-1);
                        }
                        else
                        {
                            while (chDt.Day != 1) chDt = chDt.AddDays(1);
                        }
                    }
            

            chDt = new DateTime(chDt.Year, chDt.Month, chDt.Day, 0, 0, 0);
            DateTime chDte = new DateTime(enddate.Year, enddate.Month, enddate.Day, 0, 0, 0);

            int nn = 0;

            while (chDt <= chDte)
            {
                total = 0;
                totala = 0;
                totalr = 0;
                totalsum = 0M;
                totalsuma = 0M;
                totalsumr = 0M;
                totalsumd = 0M;
                totalsumdd = 0M;
                clicks = 0;
                customclicks = 0;
                shows = 0;
                expenses = 0M;
                customexp = 0M;

                nn++;

                DateTime dd = chDt.AddDays(1);

                if (groupby == 2)
                {
                    dd = chDt.AddDays(6);
                }
                else
                    if (groupby == 3)
                    {
                        dd = chDt.AddMonths(1);
                        while (dd.Month != chDt.Month) dd = dd.AddDays(-1);
                    }

                foreach (APIServiceProviderNamespace.main.projectsRow pr in dt.Rows)
                {
                    if (pid > 0 && pr.id != pid) continue;

                    APIServiceProviderNamespace.main.ordersTotalsDataTable dt1 = DBModule.GetOrdersTotals(chDt, new DateTime(dd.Year, dd.Month, dd.Day, 23, 59, 59), pr.id, city, source, payment, days);
                    
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

                    APIServiceProviderNamespace.main.statTotalsDataTable dt2 = DBModule.GetStatTotals(chDt, new DateTime(dd.Year, dd.Month, dd.Day, 23, 59, 59), pr.id, 0, days, city);
                    
                    if (dt2.Rows.Count > 0)
                    {
                        clicks += Convert.ToInt32(dt2.Rows[0]["clicks"]);
                        customclicks += Convert.ToInt32(dt2.Rows[0]["customclicks"]);
                        shows += Convert.ToInt32(dt2.Rows[0]["shows"]);
                        expenses += Convert.ToDecimal(dt2.Rows[0]["expenses"]);
                        customexp += Convert.ToDecimal(dt2.Rows[0]["customexp"]);
                    }
                }

                dcCurrent = new DataColumn("col" + (dtChart.Columns.Count + 1).ToString(), typeof(decimal));
                dtChart.Columns.Add(dcCurrent);

                int t1 = chDt.Day;

                if (groupby == 1)
                {
                    dcCurrent.ExtendedProperties["month"] = chDt.Month;
                    dcCurrent.ExtendedProperties["title"] = chDt.ToString("dd.MM.yyyy") + " " + chDt.DayOfWeek.ToString();
                }
                else
                    if (groupby == 2)
                    {
                        CultureInfo ciCurr = CultureInfo.CurrentCulture;
                        t1 = ciCurr.Calendar.GetWeekOfYear(chDt, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);

                        dcCurrent.ExtendedProperties["month"] = chDt.Year;

                        dcCurrent.ExtendedProperties["title"] = chDt.ToString("dd.MM.yyyy") + "-" + dd.ToString("dd.MM.yyyy");
                    }
                    else
                    {
                        dcCurrent.ExtendedProperties["month"] = chDt.Year;
                        t1 = chDt.Month;
                        dcCurrent.ExtendedProperties["title"] = chDt.ToString("dd.MM.yyyy") + "-" + dd.ToString("dd.MM.yyyy");
                    }

                if (t1 >= 10)
                    dcCurrent.ExtendedProperties["title1"] = t1.ToString();
                else
                    dcCurrent.ExtendedProperties["title1"] = "0" + t1.ToString();

                if (nn <= 30)
                    dcCurrent.ExtendedProperties["left"] = false;
                else
                    dcCurrent.ExtendedProperties["left"] = true;

                dcCurrent.ExtendedProperties["total"] = total;
                dcCurrent.ExtendedProperties["totala"] = totala;
                dcCurrent.ExtendedProperties["totalr"] = totalr;
                dcCurrent.ExtendedProperties["totalsum"] = (totalsum);
                dcCurrent.ExtendedProperties["totalsuma"] = (totalsumd);
                dcCurrent.ExtendedProperties["totalsumr"] = (totalsumr);
                dcCurrent.ExtendedProperties["clicks"] = (clicks);
                dcCurrent.ExtendedProperties["googleclicks"] = (customclicks);
                dcCurrent.ExtendedProperties["yandexclicks"] = (clicks - customclicks);
                dcCurrent.ExtendedProperties["expenses"] = (expenses);
                dcCurrent.ExtendedProperties["google"] = (customexp);
                dcCurrent.ExtendedProperties["yandex"] = (expenses - customexp);
                if (totalsumd > 0)
                    dcCurrent.ExtendedProperties["percent"] = Convert.ToInt32(expenses * 100 / totalsumd);
                else
                    if (expenses > 0)
                        dcCurrent.ExtendedProperties["percent"] = 100;
                    else
                        dcCurrent.ExtendedProperties["percent"] = 0;

                if (chDt.DayOfWeek == DayOfWeek.Saturday || chDt.DayOfWeek == DayOfWeek.Sunday || DBModule.IsNotWorkingDay(chDt))
                    dcCurrent.ExtendedProperties["bg"] = "#eeeeee";
                else
                    dcCurrent.ExtendedProperties["bg"] = "#ffffff";


                if (groupby == 2)
                {
                    int dow = (int)chDt.DayOfWeek;
                    if (dow == 0) dow = 7;
                    int days1 = 7 - dow + 1;
                    chDt = chDt.AddDays(days1);
                }
                else
                    if (groupby == 1)
                        chDt = chDt.AddDays(1);
                    else
                        if (groupby == 3)
                        {
                            chDt = chDt.AddMonths(1);
                        }
            }


            while (startdate < enddate || (startdate <= enddate && groupby == 1))
            {
                records++;

                startdate = new DateTime(startdate.Year, startdate.Month, startdate.Day, 0, 0, 0);
                                      
                ed = startdate;
           

                if (groupby == 1)
                {

                }
                else
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

                total = 0;
                totala = 0;
                totalr = 0;
                totalsum = 0M;
                totalsuma = 0M;
                totalsumr = 0M;
                totalsumd = 0M;
                totalsumdd = 0M;
                clicks = 0;
                customclicks = 0;
                shows = 0;
                expenses = 0M;
                customexp = 0M;

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

                    APIServiceProviderNamespace.main.ordersTotalsDataTable dt1 = DBModule.GetOrdersTotals(startdate, ed, pr.id, city, source, payment, days);
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

                    APIServiceProviderNamespace.main.statTotalsDataTable dt2 = DBModule.GetStatTotals(startdate, ed, pr.id, 0, days, city);

                    if (dt2.Rows.Count > 0)
                    {
                        clicks += Convert.ToInt32(dt2.Rows[0]["clicks"]);
                        customclicks += Convert.ToInt32(dt2.Rows[0]["customclicks"]);
                        shows += Convert.ToInt32(dt2.Rows[0]["shows"]);
                        expenses += Convert.ToDecimal(dt2.Rows[0]["expenses"]);
                        customexp += Convert.ToDecimal(dt2.Rows[0]["customexp"]);
                    }

                    /*APIServiceProviderNamespace.main.campaignsDataTable cdt = DBModule.GetCampaignsByProjectID(pr.id);

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
                    }*/
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
                griddata += "row[\"expenses\"] = \"" + (expenses).ToString("### ### ###") + " р." + "\";";
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
                    xmldata += "<cell>" + (expenses).ToString("### ### ###") + " р." + "</cell>\n";
                else
                    xmldata += "<cell>0 р.</cell>\n";
                xmldata += "</row>\n";

                totalsums += totalsum.ToString().Replace(",", ".") + ",";
                totalsumas += totalsuma.ToString().Replace(",", ".") + ",";
                totalsumrs += totalsumr.ToString().Replace(",", ".") + ",";
                totalsumds += totalsumd.ToString().Replace(",", ".") + ",";
                totalsumdds += totalsumdd.ToString().Replace(",", ".") + ",";
                expensess += (expenses).ToString().Replace(",", ".") + ",";

                if (totalsumd > 0)
                    percents += Math.Round(Convert.ToDecimal(expenses * 100 / totalsumd), 1).ToString().Replace(",", ".") + ",";
                else
                    if (expenses > 0)
                        percents += "100,";
                    else
                        percents += "0,";

                if (groupby == 2)
                {
                    int dow = (int)startdate.DayOfWeek;
                    if (dow == 0) dow = 7;
                    int days1 = 7 - dow + 1;
                    startdate = startdate.AddDays(days1);
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
            percents = percents.Remove(percents.Length - 1);

           /* chartdata1 += "{\r\n";
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
            chartdata1 += "}";*/

            chartdata1 += "{\r\n";
            chartdata1 += "name: '% расходов от заказов',\r\n";
            chartdata1 += "data: [" + percents + "]\r\n";
            chartdata1 += "}";


            linksjs += "$('#navigate').attr('href', 'Default.aspx?sd=' + $('#startdate').val() + '&ed=' + $('#enddate').val() + '&pid=' + $('#projects').val() + '&g=' + $('#groups').val() + '&city=' + $('#city').val() + '&source=' + $('#source').val() + '&payment=' + $('#payment').val() + '&days=' + $('#days').val());";

            linksjs += "$('#edit').attr('href', 'dataedit.aspx?sd=' + $('#startdate').val() + '&ed=' + $('#enddate').val() + '&pid=' + $('#projects').val());";

            //linksjs += "jQuery('#datagrid').jqGrid({ url: 'datagen.aspx?sd=" + st.ToString("dd.MM.yyyy") + "&ed=" + ed.ToString("dd.MM.yyyy") + "&pid=" + pid.ToString() + "' });";

            string js = System.IO.File.ReadAllText(path + "\\ui\\ui_tpl.js");

            js = js.Replace("{[(CONT1)]}", "container").Replace("{[(TITLE1)]}", "График за период времени").Replace("{[(SUBTITLE1)]}", st.ToString("dd.MM.yyyy") + "-" + ed.ToString("dd.MM.yyyy")).Replace("{[(CAT1)]}", cat1).Replace("{[(DATA1)]}", chartdata1).Replace("{[(LINKS)]}", linksjs);
            //js = js.Replace("{[(GDATA)]}", griddata);


           /* string chData = "GenerateChart('chart', ";
            string cols = "[";
            string data = "[";
            foreach (DataColumn dc in dtChart.Columns)
            {
                cols += "'" + dc.ExtendedProperties["title"].ToString() + "',";
                data += "{";
               // if (Convert.ToDecimal(dc.ExtendedProperties["totalsum"]) > 0)
                {
                    data += "totala_p:55,";// + Convert.ToInt32(Convert.ToDecimal(dc.ExtendedProperties["totalsuma"]) * 100 / Convert.ToDecimal(dc.ExtendedProperties["totalsum"])).ToString() + ",";
                    data += "totalr_p:45,";// + Convert.ToInt32(Convert.ToDecimal(dc.ExtendedProperties["totalsumr"]) * 100 / Convert.ToDecimal(dc.ExtendedProperties["totalsum"])).ToString();
                }
               // else
               // {
               //     data += "totala: 0, totalr: 0";
               // }

                data += "yandex_p:30,";
                data += "google_p:70";
                data += "},";
            }

            cols = cols.Remove(cols.Length - 1);
            data = data.Remove(data.Length - 1);

            cols += "]";
            data += "]";

            chData += cols + "," + data + ");";

            
            js = System.IO.File.ReadAllText(path + "\\ui\\chartnew_tpl.htm");
            js = js.Replace("{[(FCALL)]}", chData);

            System.IO.File.WriteAllText(path + "\\ui\\chartnew.htm", js);*/

            string chData = "<table border=\"0\" cellpadding=\"0\" cellspacing=\"1\" style=\"cursor:default; background-color:#cccccc; border:1px solid #cccccc;\" id=\"statsTable\">";
            int n = 0;

            chData += "<tr>";

            int cm = (int)dtChart.Columns[0].ExtendedProperties["month"];

            int colspan = 0;

            foreach (DataColumn dc in dtChart.Columns)
            {
                if (cm != Convert.ToInt32(dc.ExtendedProperties["month"]))
                {
                    chData += "<td colspan=\"" + colspan.ToString() + "\" style=\"background-color: #ffffff; color: #979797; font-size: 13px;padding: 4px;text-align: left;color: black;\">";
                    if (groupby == 1)
                        chData += "<div>" + months[cm - 1] + "</div>";
                    else
                        chData += "<div>" + cm.ToString() + "</div>";
                    chData += "</td>";
                    cm = Convert.ToInt32(dc.ExtendedProperties["month"]);
                    colspan = 0;
                }
                colspan++;
            }

            chData += "<td colspan=\"" + colspan.ToString() + "\" style=\"background-color: #ffffff; color: #979797; font-size: 13px;padding: 4px;text-align: left;color: black;\">";
            if (groupby == 1)
                chData += "<div>" + months[cm - 1] + "</div>";
            else
                chData += "<div>" + cm.ToString() + "</div>";
            chData += "</td>";

            chData += "</tr>";

            while (n <= 4)
            {
                chData += "<tr>";

                foreach (DataColumn dc in dtChart.Columns)
                {
                    int pa = 0;
                    if (Convert.ToDecimal(dc.ExtendedProperties["totalsum"]) > 0)
                        pa = Convert.ToInt32(Convert.ToDecimal(dc.ExtendedProperties["totalsuma"]) * 100 / Convert.ToDecimal(dc.ExtendedProperties["totalsum"]));

                    int pr = 0;
                    if (Convert.ToDecimal(dc.ExtendedProperties["totalsum"]) > 0)
                        pr = Convert.ToInt32(Convert.ToDecimal(dc.ExtendedProperties["totalsumr"]) * 100 / Convert.ToDecimal(dc.ExtendedProperties["totalsum"]));


                    int py = 0;
                    if (Convert.ToDecimal(dc.ExtendedProperties["expenses"]) > 0)
                        py = Convert.ToInt32(Convert.ToDecimal(dc.ExtendedProperties["yandex"]) * 100 / Convert.ToDecimal(dc.ExtendedProperties["expenses"]));

                    int pg = 0;
                    if (Convert.ToDecimal(dc.ExtendedProperties["expenses"]) > 0)
                        pg = Convert.ToInt32(Convert.ToDecimal(dc.ExtendedProperties["google"]) * 100 / Convert.ToDecimal(dc.ExtendedProperties["expenses"]));

                    int dw = 13;

                    switch (n)
                    {
                        case 0:
                            chData += "<td style=\"background-color: " + dc.ExtendedProperties["bg"].ToString() + ";font-size: 13px;padding: 4px;text-align: center;color: black;\" title=\"" + dc.ExtendedProperties["title"].ToString() + "\" width=\"" + (dw + 4).ToString() + "\">";
                            chData += "<div>";
                            //if (!Convert.ToBoolean(dcCurrent.ExtendedProperties["left"]))
                                chData += "<a href=\"#\">" + dc.ExtendedProperties["title1"].ToString() + "</a>";
                            chData += "</div>";
                            chData += "</td>";
                            break;
                        case 1:
                            {
                                int hh = Convert.ToInt32(dc.ExtendedProperties["total"]) / 10;
                                if (hh == 0) hh = 1;
                                chData += "<td style=\"vertical-align: bottom; text-align: center; padding-top:20px; background-color: " + dc.ExtendedProperties["bg"].ToString() + "\" title=\"" + dc.ExtendedProperties["title"].ToString() + "\">";
                                if (Convert.ToInt32(dc.ExtendedProperties["totala"]) > 0 && Convert.ToInt32(dc.ExtendedProperties["totalr"]) > 0)
                                    chData += "<div  style=\"height:" + hh.ToString() + "px;background-color: #c2c2c2;width: " + dw.ToString() + "px;text-align: center;vertical-align: top;font-size:10px;color:White;padding:2px;margin-left:auto;margin-right:auto;\">&nbsp;" + /*dc.ExtendedProperties["total"].ToString() + */"</div>";

                                hh = Convert.ToInt32(dc.ExtendedProperties["totala"]) / 10;
                                if (hh == 0) hh = 1;
                                if (Convert.ToInt32(dc.ExtendedProperties["totala"]) > 0)
                                    chData += "<div  style=\"height:" + hh.ToString() + "px;background-color: #b00000;width: " + dw.ToString() + "px;text-align: center;vertical-align: top;font-size:10px;color:White;padding:2px;margin-left:auto;margin-right:auto;\">&nbsp;" + /*dc.ExtendedProperties["totala"].ToString() +*/ "</div>";

                                hh = Convert.ToInt32(dc.ExtendedProperties["totalr"]) / 10;
                                if (hh == 0) hh = 1;
                                if (Convert.ToInt32(dc.ExtendedProperties["totalr"]) > 0)
                                    chData += "<div  style=\"height:" + hh.ToString() + "px;background-color: #3f3f3f;width: " + dw.ToString() + "px;text-align: center;vertical-align: top;font-size:10px;color:White;padding:2px;margin-left:auto;margin-right:auto;\">&nbsp;" + /*dc.ExtendedProperties["totalr"].ToString() +*/ "</div>";

                                chData += "</td>";
                            }
                            break;
                        case 2:
                            {
                                chData += "<td style=\"background-color: " + dc.ExtendedProperties["bg"].ToString() + ";padding-bottom:20px;vertical-align: top; text-align: center;\" title=\"" + dc.ExtendedProperties["title"].ToString() + "\">";
                                //                            chData += "<div style=\"height:" + Convert.ToInt32((Math.Ceiling(Convert.ToDouble(dc.ExtendedProperties["expenses"]) / 1000.0)) + 10).ToString() + "px;background-color: #FF9999;width: " + dw.ToString() + "px;text-align: center;vertical-align: top;font-size:11px;color:White;padding:2px;\">" + Convert.ToDecimal(dc.ExtendedProperties["expenses"]).ToString("### ### ###") + "</div>";
                                //                            chData += "<div style=\"height:" + ((pg + 1) * 2).ToString() + "px;background-color: #FF9900;width: " + dw.ToString() + "px;text-align: center;vertical-align: top;font-size:11px;color:White;padding:2px;\">" + Convert.ToDecimal(dc.ExtendedProperties["google"]).ToString("### ### ###") + "</div>";
                                //                            chData += "<div style=\"height:" + ((py + 1) * 2).ToString() + "px;background-color: #3300FF;width: " + dw.ToString() + "px;text-align: center;vertical-align: top;font-size:11px;color:White;padding:2px;\">" + Convert.ToDecimal(dc.ExtendedProperties["yandex"]).ToString("### ### ###") + "</div>";

                                decimal k = 200.0M;

                                if (groupby == 2 || groupby == 3)
                                    k = 5000.0M;

                                decimal yd = Convert.ToDecimal(dc.ExtendedProperties["yandex"]);
                                decimal gg = Convert.ToDecimal(dc.ExtendedProperties["google"]);
                                decimal ex = Convert.ToDecimal(dc.ExtendedProperties["expenses"]);

                                if (yd > 0)
                                {
                                    /*if (yd < 10000M)
                                        k = 100M;
                                    else
                                        if (yd >= 10000 && yd < 100000)
                                            k = 1000M;
                                        else
                                            if (yd >= 100000 && yd < 1000000)
                                                k = 10000M;
                                            else
                                                k = 10000M;*/
                                    
                                    chData += "<div style=\"height:" + Convert.ToInt32(yd / k).ToString() + "px;background-color: #dbbf00;width: " + dw.ToString() + "px;text-align: center;vertical-align: top;font-size:10px;color:White;padding:2px;margin-left:auto;margin-right:auto;\">&nbsp;</div>";
                                }
                                if (gg > 0)
                                {
                                   /* if (gg < 10000M)
                                        k = 100M;
                                    else
                                        if (gg >= 10000 && gg < 100000)
                                            k = 1000M;
                                        else
                                            if (gg >= 100000 && gg < 1000000)
                                                k = 10000M;
                                            else
                                                k = 10000M;*/
                                    chData += "<div style=\"height:" + Convert.ToInt32(gg / k).ToString() + "px;background-color: #2b9900;width: " + dw.ToString() + "px;text-align: center;vertical-align: top;font-size:10px;color:White;padding:2px;margin-left:auto;margin-right:auto;\">&nbsp;</div>";
                                }
                                if (gg > 0 && yd > 0)
                                {
                                    /*if (ex < 10000M)
                                        k = 100M;
                                    else
                                        if (ex >= 10000 && ex < 100000)
                                            k = 1000M;
                                        else
                                            if (ex >= 100000 && ex < 1000000)
                                                k = 10000M;
                                            else
                                                k = 10000M;*/


                                    chData += "<div style=\"height:" + Convert.ToInt32(ex / k).ToString() + "px;background-color: #949494;width: " + dw.ToString() + "px;text-align: center;vertical-align: top;font-size:10px;color:White;padding:2px;margin-left:auto;margin-right:auto;\">&nbsp;</div>";
                                }

                                chData += "</td>";
                            }
                            break;
                        case 3:
                            {
                                chData += "<td style=\"background-color: " + dc.ExtendedProperties["bg"].ToString() + ";padding-bottom:2px;vertical-align: bottom; text-align: center;\" title=\"" + dc.ExtendedProperties["title"].ToString() + "\">";
                                //                            chData += "<div style=\"height:" + Convert.ToInt32((Math.Ceiling(Convert.ToDouble(dc.ExtendedProperties["expenses"]) / 1000.0)) + 10).ToString() + "px;background-color: #FF9999;width: " + dw.ToString() + "px;text-align: center;vertical-align: top;font-size:11px;color:White;padding:2px;\">" + Convert.ToDecimal(dc.ExtendedProperties["expenses"]).ToString("### ### ###") + "</div>";
                                //                            chData += "<div style=\"height:" + ((pg + 1) * 2).ToString() + "px;background-color: #FF9900;width: " + dw.ToString() + "px;text-align: center;vertical-align: top;font-size:11px;color:White;padding:2px;\">" + Convert.ToDecimal(dc.ExtendedProperties["google"]).ToString("### ### ###") + "</div>";
                                //                            chData += "<div style=\"height:" + ((py + 1) * 2).ToString() + "px;background-color: #3300FF;width: " + dw.ToString() + "px;text-align: center;vertical-align: top;font-size:11px;color:White;padding:2px;\">" + Convert.ToDecimal(dc.ExtendedProperties["yandex"]).ToString("### ### ###") + "</div>";

                                if (Convert.ToDouble(dc.ExtendedProperties["percent"]) > 0)
                                    chData += "<div style=\"height:" + Convert.ToInt32(Convert.ToDouble(dc.ExtendedProperties["percent"]) / 10).ToString() + "px;background-color: #91b6d3;width: " + dw.ToString() + "px;text-align: center;vertical-align: top;font-size:10px;color:White;padding:2px;margin-left:auto;margin-right:auto;\">&nbsp;</div>";

                                chData += "</td>";
                            }
                            break;
                        case 4:
                            {
                                chData += "<td style=\"background-color: " + dc.ExtendedProperties["bg"].ToString() + ";font-size: 13px;padding: 4px;text-align: center;color: black;\" title=\"" + dc.ExtendedProperties["title"].ToString() + "\" width=\"" + (dw + 4).ToString() + "\">";
                                chData += "<div>";
                                //if (!Convert.ToBoolean(dcCurrent.ExtendedProperties["left"]))
                                chData += "<div class=\"sum\" style=\"display:none;position: absolute;z-index: 1000;background-color: white;padding-left: 15px;padding-top: 5px;border: 1px solid #666666;color: black;text-align: left;\">";
                                chData += dc.ExtendedProperties["title"].ToString() + "<br/><br/>";
                                chData += "<table>";
                                chData += "<tr><td style=\"width: 100px\"><span style=\"color: #c2c2c2\">Всего заказов</span></td><td style=\"width: 35px\"><span style=\"color: #c2c2c2\">" + dc.ExtendedProperties["total"].ToString() + "</span></td><td style=\"width: 90px\"><span style=\"color: #c2c2c2\">&nbsp;на&nbsp;" + Convert.ToDecimal(dc.ExtendedProperties["totalsum"]).ToString("### ### ###").Replace(",", ".") + "&nbsp;р.</span></td></tr>";
                                chData += "<tr><td style=\"width: 100px\"><span style=\"color: #b00000\">Доставлено</span></td><td style=\"width: 35px\"><span style=\"color: #b00000\">" + dc.ExtendedProperties["totala"].ToString() + "</span></td><td style=\"width: 90px\"><span style=\"color: #b00000\">&nbsp;на&nbsp;" + Convert.ToDecimal(dc.ExtendedProperties["totalsuma"]).ToString("### ### ###").Replace(",", ".") + "&nbsp;р.</span></td></tr>";
                                chData += "<tr><td style=\"width: 100px\"><span style=\"color: #3f3f3f\">Отказов</span></td><td style=\"width: 35px\"><span style=\"color: #3f3f3f\">" + dc.ExtendedProperties["totalr"].ToString() + "</span></td><td style=\"width: 90px\"><span style=\"color: #3f3f3f\">&nbsp;на&nbsp;" + Convert.ToDecimal(dc.ExtendedProperties["totalsumr"]).ToString("### ### ###").Replace(",", ".") + "&nbsp;р.</span></td></tr>";
                                chData += "</table><br/>";

                                chData += "<table>";
                                chData += "<tr><td style=\"width: 100px\"><span style=\"color: #dbbf00\"><u>yandex.direct</u></span></td><td style=\"width: 35px\"><span style=\"color: #dbbf00\">" + dc.ExtendedProperties["yandexclicks"].ToString() + "</span></td><td style=\"width: 90px\"><span style=\"color: #dbbf00\">" + Convert.ToDecimal(dc.ExtendedProperties["yandex"]).ToString("### ### ###").Replace(",", ".") + "&nbsp;р.</span></td></tr>";
                                chData += "<tr><td style=\"width: 100px\"><span style=\"color: #2b9900\"><u>google.adwords</u></span></td><td style=\"width: 35px\"><span style=\"color: #2b9900\">" + dc.ExtendedProperties["googleclicks"].ToString() + "</span></td><td style=\"width: 90px\"><span style=\"color: #2b9900\">" + Convert.ToDecimal(dc.ExtendedProperties["google"]).ToString("### ### ###").Replace(",", ".") + "&nbsp;р.</span></td></tr>";
                                chData += "<tr><td style=\"width: 100px\"><span style=\"color: #949494\"><u>Всего расходов</u></span></td><td style=\"width: 35px\"><span style=\"color: #949494\">" + dc.ExtendedProperties["clicks"].ToString() + "</td><td style=\"width: 90px\"><span style=\"color: #949494\">" + Convert.ToDecimal(dc.ExtendedProperties["expenses"]).ToString("### ### ###").Replace(",", ".") + "&nbsp;р.</span></td></tr>";
                                chData += "</table><br/>";

                                chData += "<table>";
                                if (Convert.ToDecimal(dc.ExtendedProperties["totalsuma"]) > 0)
                                    chData += "<tr><td style=\"width: 140px\"><span style=\"color: #91b6d3\">% расходов от заказов</span></td><td><span style=\"color: #91b6d3\">" + Convert.ToInt32(Convert.ToDecimal(dc.ExtendedProperties["expenses"]) * 100 / Convert.ToDecimal(dc.ExtendedProperties["totalsuma"])).ToString() + "%</span></td></td></tr>";
                                else
                                    if (Convert.ToDecimal(dc.ExtendedProperties["expenses"]) > 0)
                                        chData += "<tr><td style=\"width: 140px\"><span style=\"color: #91b6d3\">% расходов от заказов</span></td><td><span style=\"color: #91b6d3\">100%</span></td></td></tr>";
                                    else
                                        chData += "<tr><td style=\"width: 140px\"><span style=\"color: #91b6d3\">% расходов от заказов</span></td><td><span style=\"color: #91b6d3\">0%</span></td></td></tr>";
                                chData += "</table><br/>";

                                chData += "</div>";
                                // if (Convert.ToBoolean(dcCurrent.ExtendedProperties["left"]))
                                //    chData += "<a href=\"#\">" + dc.ExtendedProperties["title1"].ToString() + "</a>";
                                chData += "</div>";
                                chData += "</td>";
                            }
                            break;
                    }
                }

                chData += "</tr>";
                n++;
            }

            chData += "</table>";

            //js = System.IO.File.ReadAllText(path + "\\ui\\mychart_tpl.htm");
            //js = js.Replace("{[(TBL)]}", chData);

            //System.IO.File.WriteAllText(path + "\\ui\\mychart.htm", js);

            js = js.Replace("{[(SCH)]}", chData);

            System.IO.File.WriteAllText(path + "\\ui\\ui.js", js);
            System.IO.File.WriteAllText(path + "\\ui\\griddata.xml", xmldata);

            Response.ContentType = "text/xml";
            Response.Write(xmldata);
            //Response.Redirect("http://xstat.fitmedia.ru");
            Response.Redirect("index.htm");
        }
    }
}
