using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;
using APIServiceProviderNamespace;
using System.IO;
using System.Reflection;
using System.Globalization;

namespace fitmedia
{
    public partial class _Default : System.Web.UI.Page
    {
        protected void gvData_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvData.PageIndex = e.NewPageIndex;
        }

        protected void gvData_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                DataRowView rowView = (DataRowView)e.Row.DataItem;

                for (int i = 4; i <= 8; i++ )
                    if (Convert.ToDecimal(rowView[i]) > 0)
                        e.Row.Cells[i].Text = string.Format("{0:### ### ###} р.", Convert.ToDecimal(rowView[i]));
                    else
                    {
                        e.Row.Cells[i].Text = "0 р.";
                    }


                if (Convert.ToDecimal(rowView[11]) > 0)
                    e.Row.Cells[11].Text = string.Format("{0:### ### ###} р.", Convert.ToDecimal(rowView[11]));
                else
                {
                    e.Row.Cells[11].Text = "0 р.";
                }
            }
        }


        private void FillGrid(Hashtable projectids, Hashtable campaignids)
        {
            if (projectids == null || campaignids == null) return;

            DataTable datatable = new DataTable();

            /*datatable.Columns.Add(new DataColumn("Период", typeof(string)));
            datatable.Columns.Add(new DataColumn("Заказы", typeof(int)));
            datatable.Columns.Add(new DataColumn("Под. заказы", typeof(int)));
            datatable.Columns.Add(new DataColumn("Отказы", typeof(int)));

            datatable.Columns.Add(new DataColumn("Общ. сумма заказов", typeof(decimal)));
            datatable.Columns.Add(new DataColumn("Сумма под. заказов", typeof(decimal)));
            datatable.Columns.Add(new DataColumn("Сумма отказов", typeof(decimal)));
            datatable.Columns.Add(new DataColumn("Реал. сумма заказов", typeof(decimal)));
            datatable.Columns.Add(new DataColumn("Сумма скидок", typeof(decimal)));

            datatable.Columns.Add(new DataColumn("Клики", typeof(int)));
            datatable.Columns.Add(new DataColumn("Показы", typeof(int)));
            datatable.Columns.Add(new DataColumn("Расходы", typeof(int)));*/


            datatable.Columns.Add(new DataColumn("period", typeof(string)));
            datatable.Columns.Add(new DataColumn("total", typeof(int)));
            datatable.Columns.Add(new DataColumn("totala", typeof(int)));
            datatable.Columns.Add(new DataColumn("totalr", typeof(int)));

            datatable.Columns.Add(new DataColumn("totalsum", typeof(decimal)));
            datatable.Columns.Add(new DataColumn("totalsuma", typeof(decimal)));
            datatable.Columns.Add(new DataColumn("totalsumr", typeof(decimal)));
            datatable.Columns.Add(new DataColumn("totalsumd", typeof(decimal)));
            datatable.Columns.Add(new DataColumn("totalsumdd", typeof(decimal)));

            datatable.Columns.Add(new DataColumn("clicks", typeof(int)));
            datatable.Columns.Add(new DataColumn("shows", typeof(int)));
            datatable.Columns.Add(new DataColumn("expenses", typeof(decimal)));

            APIServiceProviderNamespace.main.projectsDataTable dt = DBModule.GetProjects();

            if (dt == null) return;

            DateTime startdate = (DateTime)Page.Session["startdate"];//new DateTime(2012, 1, 1);//DateTime.Now;
            DateTime enddate = (DateTime)Page.Session["enddate"];//DateTime.Now;

            
            while (startdate.DayOfWeek != DayOfWeek.Monday && startdate.Month >= 1 && startdate.Day > 1)
                startdate = startdate.AddDays(-1);
            while (enddate.DayOfWeek != DayOfWeek.Sunday)
                enddate = enddate.AddDays(1);

            DateTime st = startdate;
            DateTime ed = enddate;

            int total_g = 0;
            int totala_g = 0;
            int totalr_g = 0;
            decimal totalsum_g = 0;
            decimal totalsuma_g = 0;
            decimal totalsumr_g = 0;
            decimal totalsumd_g = 0;
            decimal totalsumdd_g = 0;
            int clicks_g = 0;
            int shows_g = 0;
            decimal expenses_g = 0;


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

            while (startdate < enddate)
            {
                ed = startdate;
                while (ed.DayOfWeek != DayOfWeek.Sunday )
                    ed = ed.AddDays(1);
                
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

                foreach (APIServiceProviderNamespace.main.projectsRow pr in dt.Rows)
                {
                    if (projectids[pr.id] == null)
                        continue;

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
                        if (campaignids[cr.id] == null)
                            continue;

                        APIServiceProviderNamespace.main.statTotalsDataTable dt2 = DBModule.GetStatTotals(startdate, ed, pr.id, cr.id);

                        if (dt2.Rows.Count > 0)
                        {
                            clicks += Convert.ToInt32(dt2.Rows[0]["clicks"]);
                            shows += Convert.ToInt32(dt2.Rows[0]["shows"]);
                            expenses += Convert.ToDecimal(dt2.Rows[0]["expenses"]);
                        }
                    }
                }

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

                DataRow row = datatable.NewRow();
                row["period"] = startdate.ToString("dd.MM.yyyy") + " - " + ed.ToString("dd.MM.yyyy");
                row["total"] = total;
                row["totala"] = totala;
                row["totalr"] = totalr;
                row["totalsum"] = totalsum;
                row["totalsuma"] = totalsuma;
                row["totalsumr"] = totalsumr;
                row["totalsumd"] = totalsumd;
                row["totalsumdd"] = totalsumdd;
                row["clicks"] = clicks;
                row["shows"] = shows;
                row["expenses"] = expenses;

                cat1 += "'" + startdate.ToString("dd.MM.yyyy") + "-" + ed.ToString("dd.MM.yyyy") + "'";
                cat1 += ",";

                totalsums += totalsum.ToString().Replace(",", ".") + ",";
                totalsumas += totalsuma.ToString().Replace(",", ".") + ",";
                totalsumrs += totalsumr.ToString().Replace(",", ".") + ",";
                totalsumds += totalsumd.ToString().Replace(",", ".") + ",";
                totalsumdds += totalsumdd.ToString().Replace(",", ".") + ",";
                expensess += expenses.ToString().Replace(",", ".") + ",";

                total_g += total;
                totala_g += totala;
                totalr_g += totalr;
                totalsum_g += totalsum;
                totalsuma_g += totalsuma;
                totalsumr_g += totalsumr;
                totalsumd_g += totalsumd;
                totalsumdd_g += totalsumdd;
                clicks_g += clicks;
                shows_g += shows;
                expenses_g += expenses;

                datatable.Rows.Add(row);
                int dow = (int)startdate.DayOfWeek;
                if (dow == 0) dow = 7;
                int days =  7 - dow + 1;
                startdate = startdate.AddDays(days);
            }

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
            chartdata1 += "name: 'Реал. сумма заказов',\r\n";
            chartdata1 += "data: [" + totalsumds + "]\r\n";
            chartdata1 += "},\r\n";
         
            chartdata1 += "{\r\n";
            chartdata1 += "name: 'Сумма скидок',\r\n";
            chartdata1 += "data: [" + totalsumdds + "]\r\n";
            chartdata1 += "},";

            chartdata1 += "{\r\n";
            chartdata1 += "name: 'Расходы',\r\n";
            chartdata1 += "data: [" + expensess + "]\r\n";
            chartdata1 += "}";

          /*  NumberFormatInfo nfi = (NumberFormatInfo)CultureInfo.InvariantCulture.NumberFormat.Clone();
            nfi.CurrencyGroupSeparator = " ";
            nfi.CurrencyDecimalSeparator = ".";
            nfi.CurrencyDecimalDigits = 1;
            nfi.CurrencySymbol = "";*/
          // totalsum_g = 155698745;


            /*gvData.Columns[0].FooterText = "Всего";
            gvData.Columns[1].FooterText = total_g.ToString();
            gvData.Columns[2].FooterText = totala_g.ToString();
            gvData.Columns[3].FooterText = totalr_g.ToString();
            if (totalsum_g > 0)
                gvData.Columns[4].FooterText = totalsum_g.ToString("### ### ###") + " р.";
            else
                gvData.Columns[4].FooterText = "0 р.";
            if (totalsuma_g > 0)
                gvData.Columns[5].FooterText = totalsuma_g.ToString("### ### ###") + " р.";
            else
                gvData.Columns[5].FooterText = "0 р.";
            if (totalsumr_g > 0)
                gvData.Columns[6].FooterText = totalsumr_g.ToString("### ### ###") + " р.";
            else
                gvData.Columns[6].FooterText = "0 р.";
            if (totalsumd_g > 0)
                gvData.Columns[7].FooterText = totalsumd_g.ToString("### ### ###") + " р.";
            else
                gvData.Columns[7].FooterText = "0 р.";
            if (totalsumdd_g > 0)
                gvData.Columns[8].FooterText = totalsumdd_g.ToString("### ### ###") + " р.";
            else
                gvData.Columns[8].FooterText = "0 р.";
            gvData.Columns[9].FooterText = clicks_g.ToString();
            gvData.Columns[10].FooterText = shows_g.ToString();
            if (expenses_g > 0)
                gvData.Columns[11].FooterText = expenses_g.ToString("### ### ###") + " р.";
            else
                gvData.Columns[11].FooterText = "0 р.";*/


            /*GridView1.Columns.FromDataField("period").FooterValue = "Total";
            JQGrid1.Columns.FromDataField("total").FooterValue = total_g.ToString();
            JQGrid1.Columns.FromDataField("totala").FooterValue = totala_g.ToString();
            JQGrid1.Columns.FromDataField("totalr").FooterValue = totalr_g.ToString();
            JQGrid1.Columns.FromDataField("totalsum").FooterValue = totalsum_g.ToString();
            JQGrid1.Columns.FromDataField("totalsuma").FooterValue = totalsuma_g.ToString();
            JQGrid1.Columns.FromDataField("totalsumr").FooterValue = totalsumr_g.ToString();
            JQGrid1.Columns.FromDataField("totalsumd").FooterValue = totalsumd_g.ToString();
            JQGrid1.Columns.FromDataField("totalsumdd").FooterValue = totalsumdd_g.ToString();
            JQGrid1.Columns.FromDataField("clicks").FooterValue = clicks_g.ToString();
            JQGrid1.Columns.FromDataField("shows").FooterValue = shows_g.ToString();
            JQGrid1.Columns.FromDataField("expenses").FooterValue = expenses_g.ToString();*/

            /*string path = Request.PhysicalApplicationPath;
            string js = System.IO.File.ReadAllText(path + "\\ui\\ui_tpl.js");
           // string jsgrid = System.IO.File.ReadAllText(path + "\\ui\\grid_tpl.js");

            if (!ClientScript.IsStartupScriptRegistered("JSScript"))
            {
                js = js.Replace("{[(CONT1)]}", "container").Replace("{[(TITLE1)]}", "График за период времени").Replace("{[(SUBTITLE1)]}", st.ToString("dd.MM.yyyy") + "-" + ed.ToString("dd.MM.yyyy")).Replace("{[(CAT1)]}", cat1).Replace("{[(DATA1)]}", chartdata1).Replace("{[(GDATA)]}", griddata);
                //js = js.Replace("{[(CONT2)]}", "container1").Replace("{[(TITLE2)]}", "График за текущую неделю").Replace("{[(DATA2)]}", chartdata1);
                ClientScript.RegisterStartupScript(this.GetType(), "JSScript", js);
            }*/
        }

        private void LoadChart()
        {
        }

        public void tvCampaigns_CheckChanged(Object sender, TreeNodeEventArgs e)
        {
            Hashtable campaignids = (Hashtable) Page.Session["campaignids"];
            if (campaignids == null)
            {
                campaignids = new Hashtable();
                Page.Session["campaignids"] = campaignids;
            }
    
            Hashtable projectids = (Hashtable)Page.Session["projectids"];
            if (projectids == null)
            {
                projectids = new Hashtable();
                Page.Session["projectids"] = projectids;
            }

            if (e.Node.Checked)
            {
                if (e.Node.Parent != null)
                {
                    campaignids[Convert.ToInt32(e.Node.Value)] = e.Node.Value;
                    projectids[Convert.ToInt32(e.Node.Parent.Value)] = e.Node.Parent.Value;
                    e.Node.Parent.Checked = true;
                }
                else
                {
                    projectids[Convert.ToInt32(e.Node.Value)] = e.Node.Value;
                    foreach (TreeNode node in e.Node.ChildNodes)
                    {
                        node.Checked = true;
                        campaignids[Convert.ToInt32(node.Value)] = node.Value;
                    }
                }
            }
            else
            {
                if (e.Node.Parent != null)
                {
                    campaignids.Remove(Convert.ToInt32(e.Node.Value));
                    foreach (TreeNode node in e.Node.Parent.ChildNodes)
                    {
                        if (node.Checked) return;
                    }
                    projectids.Remove(Convert.ToInt32(e.Node.Parent.Value));
                    e.Node.Parent.Checked = false;
                }
                else
                {
                    projectids.Remove(Convert.ToInt32(e.Node.Value));
                    foreach (TreeNode node in e.Node.ChildNodes)
                    {
                        node.Checked = false;
                        campaignids.Remove(Convert.ToInt32(node.Value));
                    }
                }
            }
        }

        public void btnReload_Click(object sender, EventArgs e)
        {

        }

        public void Startdate_SelChange(object sender, EventArgs e)
        {
            Page.Session["startdate"] = Calendar1.SelectedDate;
        }

        public void Enddate_SelChange(object sender, EventArgs e)
        {
            Page.Session["enddate"] = Calendar2.SelectedDate;
        }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);

/*

            DateTime startdate = DateTime.Now;
            DateTime enddate = DateTime.Now;

            while (startdate.DayOfWeek != DayOfWeek.Monday)
                startdate = startdate.AddDays(-1);
            while (enddate.DayOfWeek != DayOfWeek.Sunday)
                enddate = enddate.AddDays(1);

            if (Page.Session["startdate"] != null)
                startdate = (DateTime)Page.Session["startdate"];
            else
            {
                Calendar1.TodaysDate = startdate;
                Calendar1.SelectedDate = startdate;
                Page.Session["startdate"] = startdate;
            }

            if (Page.Session["enddate"] != null)
                enddate = (DateTime)Page.Session["enddate"];
            else
            {
                Calendar2.TodaysDate = enddate;
                Calendar2.SelectedDate = enddate;
                Page.Session["enddate"] = enddate;
            }

            bool isfirst = false;

            if (Page.Session["projectids"] == null)
            {
                isfirst = true;
                Page.Session["projectids"] = new Hashtable();
            }


            if (Page.Session["campaignids"] == null)
            {
                isfirst = true;
                Page.Session["campaignids"] = new Hashtable();
            }*/

            if (!IsPostBack)
            {
                /*tvCampaigns.Nodes.Clear();

                APIServiceProviderNamespace.main.projectsDataTable dt = DBModule.GetProjects();

                foreach (APIServiceProviderNamespace.main.projectsRow pr in dt.Rows)
                {
                    TreeNode root = new TreeNode(pr.name, pr.id.ToString());
                    root.Checked = true;
                    //(Page.Session["projectids"] as Hashtable)[Convert.ToInt32(root.Value)] = root.Value;

                    tvCampaigns.Nodes.Add(root);
                    APIServiceProviderNamespace.main.campaignsDataTable cdt = DBModule.GetCampaignsByProjectID(pr.id);

                    foreach (APIServiceProviderNamespace.main.campaignsRow cr in cdt.Rows)
                    {
                        TreeNode node = new TreeNode(cr.name, cr.id.ToString());
                        root.ChildNodes.Add(node);
                        node.Checked = true;
                       //(Page.Session["campaignids"] as Hashtable)[Convert.ToInt32(node.Value)] = node.Value;
                    }
                }*/
            }

           // FillGrid((Hashtable)Page.Session["projectids"], (Hashtable)Page.Session["campaignids"]);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                object o = Page.Session["example"];
                Page.Session.Remove("startdate");
                Page.Session.Remove("enddate");
            }



        }
    }
}
