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

namespace fitmedia
{
    public partial class _Default : System.Web.UI.Page
    {
        private void FillGrid(Hashtable campaignids)
        {
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
            datatable.Columns.Add(new DataColumn("expenses", typeof(int)));

            APIServiceProviderNamespace.main.projectsDataTable dt = DBModule.GetProjects();

            if (dt == null) return;

            DateTime startdate = (DateTime)Page.Session["startdate"];//new DateTime(2012, 1, 1);//DateTime.Now;
            DateTime enddate = (DateTime)Page.Session["enddate"];//DateTime.Now;

            
            while (startdate.DayOfWeek != DayOfWeek.Monday)
                startdate = startdate.AddDays(-1);
            while (enddate.DayOfWeek != DayOfWeek.Sunday)
                enddate = enddate.AddDays(1);

            DateTime st = startdate;

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

                foreach (APIServiceProviderNamespace.main.projectsRow pr in dt.Rows)
                {
                    APIServiceProviderNamespace.main.ordersTotalsDataTable dt1 = DBModule.GetOrdersTotals(startdate, startdate.AddDays(7), pr.id);
                    //APIServiceProviderNamespace.main.ordersTotalsDataTable dt1 = DBModule.GetOrdersTotals(pr.startfrom, DateTime.Now, pr.id);

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
                        if (campaignids != null && campaignids.Count > 0 && campaignids[cr.id] == null)
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

                DataRow row = datatable.NewRow();
                row["period"] = startdate.ToShortDateString() + " - " + startdate.AddDays(7).ToShortDateString();
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
                startdate = startdate.AddDays(7);
            }

            string chartdata = "name: '" + st.ToShortDateString() + " - " + enddate.ToShortDateString() + "',\r\n";
            chartdata += "data: [";
            chartdata += total_g.ToString() + ",";
            chartdata += totala_g.ToString() + ",";
            chartdata += totalr_g.ToString() + ",";
            chartdata += totalsum_g.ToString().Replace(",", ".") + ",";
            chartdata += totalsuma_g.ToString().Replace(",", ".") + ",";
            chartdata += totalsumr_g.ToString().Replace(",", ".") + ",";
            chartdata += totalsumd_g.ToString().Replace(",", ".") + ",";
            chartdata += totalsumdd_g.ToString().Replace(",", ".") + ",";
            chartdata += clicks_g.ToString() + ",";
            chartdata += shows_g.ToString() + ",";
            chartdata += expenses_g.ToString().Replace(",", ".") + "]";

            JQGrid1.Columns.FromDataField("period").FooterValue = "Total";
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
            JQGrid1.Columns.FromDataField("expenses").FooterValue = expenses_g.ToString();

            startdate = DateTime.Now;
            enddate = DateTime.Now;

            while (startdate.DayOfWeek != DayOfWeek.Monday)
                startdate = startdate.AddDays(-1);
            while (enddate.DayOfWeek != DayOfWeek.Sunday)
                enddate = enddate.AddDays(1);


            string chartdata1 = "name: '" + startdate.ToShortDateString() + " - " + enddate.ToShortDateString() + "',\r\n";
            chartdata1 += "data: [";
            APIServiceProviderNamespace.main.ordersTotalsDataTable dtt1 = DBModule.GetOrdersTotals(startdate, enddate, 0);
            //APIServiceProviderNamespace.main.ordersTotalsDataTable dt1 = DBModule.GetOrdersTotals(pr.startfrom, DateTime.Now, pr.id);

            if (dtt1.Rows.Count > 0)
            {
                chartdata1 += dtt1.Rows[0]["total"].ToString() + ",";
                chartdata1 += dtt1.Rows[0]["totala"].ToString() + ",";
                chartdata1 += dtt1.Rows[0]["totalr"].ToString() + ",";
                chartdata1 += dtt1.Rows[0]["totalsum"].ToString().Replace(",", ".") + ",";
                chartdata1 += dtt1.Rows[0]["totalsuma"].ToString().Replace(",", ".") + ",";
                chartdata1 += dtt1.Rows[0]["totalsumr"].ToString().Replace(",", ".") + ",";
                chartdata1 += dtt1.Rows[0]["totalsumd"].ToString().Replace(",", ".") + ",";
                chartdata1 += dtt1.Rows[0]["totalsumdd"].ToString().Replace(",", ".") + ",";
            }


            APIServiceProviderNamespace.main.statTotalsDataTable dtt2 = DBModule.GetStatTotals(startdate, enddate, 0, 0);

            if (dtt2.Rows.Count > 0)
            {
                chartdata1 += dtt2.Rows[0]["clicks"].ToString() + ",";
                chartdata1 += dtt2.Rows[0]["shows"].ToString() + ",";
                chartdata1 += dtt2.Rows[0]["expenses"].ToString().Replace(",", ".") + "]";
            }


            string path = Request.PhysicalApplicationPath;
            string js = System.IO.File.ReadAllText(path + "\\ui\\chart_tpl.js");

            if (!ClientScript.IsStartupScriptRegistered("JSScript"))
            {
                js = js.Replace("{[(CONT1)]}", "container").Replace("{[(TITLE1)]}", "График за период времени").Replace("{[(DATA1)]}", chartdata);
                js = js.Replace("{[(CONT2)]}", "container1").Replace("{[(TITLE2)]}", "График за текущую неделю").Replace("{[(DATA2)]}", chartdata1);
                ClientScript.RegisterStartupScript(this.GetType(), "JSScript", js);
            }

            JQGrid1.DataSource = datatable;
            JQGrid1.DataBind();
        }

        private void LoadChart()
        {
        }

        public void tvCampaigns_CheckChanged(Object sender, TreeNodeEventArgs e)
        {
            return;
            Hashtable campaignids = new Hashtable();

            foreach (TreeNode node in tvCampaigns.Nodes)
            {
                foreach (TreeNode node1 in node.ChildNodes)
                {
                    if (node1.Checked)
                    {
                        campaignids[Convert.ToInt32(node1.Value)] = node1.Value;
                    }
                }
            }

            FillGrid(campaignids);
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
                Calendar1.SelectedDate = startdate;
                Page.Session["startdate"] = startdate;
            }

            if (Page.Session["enddate"] != null)
                enddate = (DateTime)Page.Session["enddate"];
            else
            {
                Calendar2.SelectedDate = enddate;
                Page.Session["enddate"] = enddate;
            }

            FillGrid(null);

            return;
            APIServiceProviderNamespace.main.projectsDataTable dt = DBModule.GetProjects();

            foreach (APIServiceProviderNamespace.main.projectsRow pr in dt.Rows)
            {
                TreeNode root = new TreeNode(pr.name, pr.id.ToString());

                tvCampaigns.Nodes.Add(root);
                APIServiceProviderNamespace.main.campaignsDataTable cdt = DBModule.GetCampaignsByProjectID(pr.id);

                foreach (APIServiceProviderNamespace.main.campaignsRow cr in cdt.Rows)
                {
                    TreeNode node = new TreeNode(cr.name, cr.id.ToString());
                    root.ChildNodes.Add(node);
                }
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {

            }



        }
    }
}
