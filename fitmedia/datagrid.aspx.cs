using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using APIServiceProviderNamespace;
using System.Data;

namespace fitmedia
{
    public partial class datagrid : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            DataTable accounts = new DataTable("Accounts");

            accounts.Columns.Add(new DataColumn("Account", typeof(string)));
            accounts.Columns.Add(new DataColumn("Clicks", typeof(int)));
            accounts.Columns.Add(new DataColumn("Cost", typeof(double)));
            accounts.Columns.Add(new DataColumn("Impressions", typeof(int)));
            accounts.Columns.Add(new DataColumn("CTR", typeof(string)));
            accounts.Columns.Add(new DataColumn("AvgCPC", typeof(double)));
            accounts.Columns.Add(new DataColumn("Conversions", typeof(int)));

            Random r = new Random();

            int totalclicks = 0;
            int totalimpressions = 0;

            for (int i = 1; i <= 10; i++)
            {
                DataRow row = accounts.NewRow();
                row["Account"] = "Account" + i.ToString();
                row["Clicks"] = r.Next(100);
                row["Cost"] = r.NextDouble().ToString("N2");
                row["Impressions"] = r.Next(1, 100);
                row["CTR"] = (Convert.ToDouble(row["Clicks"]) / Convert.ToDouble(row["Impressions"]) * 100.0).ToString("N2") + "%";
                row["AvgCPC"] = r.NextDouble().ToString("N2");
                row["Conversions"] = r.Next(10);
                accounts.Rows.Add(row);

                totalclicks += Convert.ToInt32(row["Clicks"]);
                totalimpressions += Convert.ToInt32(row["Impressions"]);
            }

            JQGrid1.Columns.FromDataField("Clicks").FooterValue = totalclicks.ToString();
            JQGrid1.Columns.FromDataField("Impressions").FooterValue = totalimpressions.ToString();
            JQGrid1.Columns.FromDataField("CTR").FooterValue = (((double)totalclicks) / (double)totalimpressions * 100.0).ToString("N2") + "%";

            JQGrid1.DataSource = accounts;
            JQGrid1.DataBind();


            DataTable campaigns = new DataTable("Campaigns");

            campaigns.Columns.Add(new DataColumn("Campaign", typeof(string)));
            campaigns.Columns.Add(new DataColumn("Clicks", typeof(int)));
            campaigns.Columns.Add(new DataColumn("Cost", typeof(double)));
            campaigns.Columns.Add(new DataColumn("Impressions", typeof(int)));
            campaigns.Columns.Add(new DataColumn("CTR", typeof(string)));
            campaigns.Columns.Add(new DataColumn("AvgCPC", typeof(double)));
            campaigns.Columns.Add(new DataColumn("Conversions", typeof(int)));
            campaigns.Columns.Add(new DataColumn("AdNetwork", typeof(string)));
            campaigns.Columns.Add(new DataColumn("Device", typeof(string)));
            campaigns.Columns.Add(new DataColumn("ClickType", typeof(string))); 
            

            totalclicks = 0;
            totalimpressions = 0;

            for (int i = 1; i <= 10; i++)
            {
                DataRow row = campaigns.NewRow();
                row["Campaign"] = "Campaign" + i.ToString();
                row["Clicks"] = r.Next(100);
                row["Cost"] = r.NextDouble().ToString("N2");
                row["Impressions"] = r.Next(1, 100);
                row["CTR"] = (Convert.ToDouble(row["Clicks"]) / Convert.ToDouble(row["Impressions"]) * 100.0).ToString("N2") + "%";
                row["AvgCPC"] = r.NextDouble().ToString("N2");
                row["Conversions"] = r.Next(10);
                row["AdNetwork"] = "AdNetwork" + r.Next(1, 10);
                row["Device"] = "Device" + r.Next(1, 10);
                row["ClickType"] = "ClickType" + r.Next(1, 10);                

                campaigns.Rows.Add(row);

                totalclicks += Convert.ToInt32(row["Clicks"]);
                totalimpressions += Convert.ToInt32(row["Impressions"]);
            }

            JQGrid2.Columns.FromDataField("Clicks").FooterValue = totalclicks.ToString();
            JQGrid2.Columns.FromDataField("Impressions").FooterValue = totalimpressions.ToString();
            JQGrid2.Columns.FromDataField("CTR").FooterValue = (((double)totalclicks) / (double)totalimpressions * 100.0).ToString("N2") + "%";

            JQGrid2.DataSource = campaigns;
            JQGrid2.DataBind();



            DataTable adgroups = new DataTable("AdGroups");

            adgroups.Columns.Add(new DataColumn("AdGroup", typeof(string)));
            adgroups.Columns.Add(new DataColumn("Clicks", typeof(int)));
            adgroups.Columns.Add(new DataColumn("Cost", typeof(double)));
            adgroups.Columns.Add(new DataColumn("Impressions", typeof(int)));
            adgroups.Columns.Add(new DataColumn("CTR", typeof(string)));
            adgroups.Columns.Add(new DataColumn("AvgCPC", typeof(double)));
            adgroups.Columns.Add(new DataColumn("AvgPosition", typeof(double)));
            adgroups.Columns.Add(new DataColumn("Conversions", typeof(int)));
            adgroups.Columns.Add(new DataColumn("AdNetwork", typeof(string)));
            adgroups.Columns.Add(new DataColumn("Device", typeof(string)));
            adgroups.Columns.Add(new DataColumn("ClickType", typeof(string)));


            totalclicks = 0;
            totalimpressions = 0;

            for (int i = 1; i <= 10; i++)
            {
                DataRow row = adgroups.NewRow();
                row["AdGroup"] = "AdGroup" + i.ToString();
                row["Clicks"] = r.Next(100);
                row["Cost"] = r.NextDouble().ToString("N2");
                row["Impressions"] = r.Next(1, 100);
                row["CTR"] = (Convert.ToDouble(row["Clicks"]) / Convert.ToDouble(row["Impressions"]) * 100.0).ToString("N2") + "%";
                row["AvgCPC"] = r.NextDouble().ToString("N2");
                row["AvgPosition"] = r.NextDouble().ToString("N2");
                row["Conversions"] = r.Next(10);
                row["AdNetwork"] = "AdNetwork" + r.Next(1, 10);
                row["Device"] = "Device" + r.Next(1, 10);
                row["ClickType"] = "ClickType" + r.Next(1, 10);

                adgroups.Rows.Add(row);

                totalclicks += Convert.ToInt32(row["Clicks"]);
                totalimpressions += Convert.ToInt32(row["Impressions"]);
            }

            JQGrid3.Columns.FromDataField("Clicks").FooterValue = totalclicks.ToString();
            JQGrid3.Columns.FromDataField("Impressions").FooterValue = totalimpressions.ToString();
            JQGrid3.Columns.FromDataField("CTR").FooterValue = (((double)totalclicks) / (double)totalimpressions * 100.0).ToString("N2") + "%";

            JQGrid3.DataSource = adgroups;
            JQGrid3.DataBind();



            DataTable ads = new DataTable("Ads");

            ads.Columns.Add(new DataColumn("Ad", typeof(string)));
            ads.Columns.Add(new DataColumn("Clicks", typeof(int)));
            ads.Columns.Add(new DataColumn("Cost", typeof(double)));
            ads.Columns.Add(new DataColumn("Impressions", typeof(int)));
            ads.Columns.Add(new DataColumn("CTR", typeof(string)));
            ads.Columns.Add(new DataColumn("AvgCPC", typeof(double)));
            ads.Columns.Add(new DataColumn("AvgPosition", typeof(double)));
            ads.Columns.Add(new DataColumn("Conversions", typeof(int)));
 

            totalclicks = 0;
            totalimpressions = 0;

            for (int i = 1; i <= 10; i++)
            {
                DataRow row = ads.NewRow();
                row["Ad"] = "Ad" + i.ToString();
                row["Clicks"] = r.Next(100);
                row["Cost"] = r.NextDouble().ToString("N2");
                row["Impressions"] = r.Next(1, 100);
                row["CTR"] = (Convert.ToDouble(row["Clicks"]) / Convert.ToDouble(row["Impressions"]) * 100.0).ToString("N2") + "%";
                row["AvgCPC"] = r.NextDouble().ToString("N2");
                row["AvgPosition"] = r.NextDouble().ToString("N2");
                row["Conversions"] = r.Next(10);

                ads.Rows.Add(row);

                totalclicks += Convert.ToInt32(row["Clicks"]);
                totalimpressions += Convert.ToInt32(row["Impressions"]);
            }

            JQGrid4.Columns.FromDataField("Clicks").FooterValue = totalclicks.ToString();
            JQGrid4.Columns.FromDataField("Impressions").FooterValue = totalimpressions.ToString();
            JQGrid4.Columns.FromDataField("CTR").FooterValue = (((double)totalclicks) / (double)totalimpressions * 100.0).ToString("N2") + "%";

            JQGrid4.DataSource = ads;
            JQGrid4.DataBind();


            DataTable keywords = new DataTable("Keywords");

            keywords.Columns.Add(new DataColumn("Keyword", typeof(string)));
            keywords.Columns.Add(new DataColumn("Clicks", typeof(int)));
            keywords.Columns.Add(new DataColumn("Cost", typeof(double)));
            keywords.Columns.Add(new DataColumn("Impressions", typeof(int)));
            keywords.Columns.Add(new DataColumn("CTR", typeof(string)));
            keywords.Columns.Add(new DataColumn("AvgCPC", typeof(double)));
            keywords.Columns.Add(new DataColumn("FirstPageCPC", typeof(double)));
            keywords.Columns.Add(new DataColumn("AvgPosition", typeof(double)));
            keywords.Columns.Add(new DataColumn("Conversions", typeof(int)));


            totalclicks = 0;
            totalimpressions = 0;

            for (int i = 1; i <= 10; i++)
            {
                DataRow row = keywords.NewRow();
                row["Keyword"] = "Keyword" + i.ToString();
                row["Clicks"] = r.Next(100);
                row["Cost"] = r.NextDouble().ToString("N2");
                row["Impressions"] = r.Next(1, 100);
                row["CTR"] = (Convert.ToDouble(row["Clicks"]) / Convert.ToDouble(row["Impressions"]) * 100.0).ToString("N2") + "%";
                row["AvgCPC"] = r.NextDouble().ToString("N2");
                row["FirstPageCPC"] = r.NextDouble().ToString("N2");
                row["AvgPosition"] = r.NextDouble().ToString("N2");
                row["Conversions"] = r.Next(10);

                keywords.Rows.Add(row);

                totalclicks += Convert.ToInt32(row["Clicks"]);
                totalimpressions += Convert.ToInt32(row["Impressions"]);
            }

            JQGrid5.Columns.FromDataField("Clicks").FooterValue = totalclicks.ToString();
            JQGrid5.Columns.FromDataField("Impressions").FooterValue = totalimpressions.ToString();
            JQGrid5.Columns.FromDataField("CTR").FooterValue = (((double)totalclicks) / (double)totalimpressions * 100.0).ToString("N2") + "%";

            JQGrid5.DataSource = keywords;
            JQGrid5.DataBind();



            totalclicks = 0;
            totalimpressions = 0;


            DataTable queries = new DataTable("SearchQueries");

            queries.Columns.Add(new DataColumn("QueryTerm", typeof(string)));
            queries.Columns.Add(new DataColumn("MatchType", typeof(string)));
            queries.Columns.Add(new DataColumn("Clicks", typeof(int)));
            queries.Columns.Add(new DataColumn("Cost", typeof(double)));
            queries.Columns.Add(new DataColumn("Impressions", typeof(int)));


            for (int i = 1; i <= 10; i++)
            {
                DataRow row = queries.NewRow();
                row["QueryTerm"] = "QueryTerm" + i.ToString();
                row["MatchType"] = "MatchType" + i.ToString();
                row["Clicks"] = r.Next(100);
                row["Cost"] = r.NextDouble().ToString("N2");
                row["Impressions"] = r.Next(1, 100);

                queries.Rows.Add(row);

                totalclicks += Convert.ToInt32(row["Clicks"]);
                totalimpressions += Convert.ToInt32(row["Impressions"]);

            }

            JQGrid6.Columns.FromDataField("Clicks").FooterValue = totalclicks.ToString();
            JQGrid6.Columns.FromDataField("Impressions").FooterValue = totalimpressions.ToString();

            
            JQGrid6.DataSource = queries;
            JQGrid6.DataBind();


            totalclicks = 0;
            totalimpressions = 0;


            DataTable geos = new DataTable("GeoPerofrmance");

            geos.Columns.Add(new DataColumn("Campaign", typeof(string)));
            geos.Columns.Add(new DataColumn("Country", typeof(string)));
            geos.Columns.Add(new DataColumn("Region", typeof(string)));
            geos.Columns.Add(new DataColumn("MetroArea", typeof(string)));
            geos.Columns.Add(new DataColumn("City", typeof(string)));
            geos.Columns.Add(new DataColumn("Clicks", typeof(int)));
            geos.Columns.Add(new DataColumn("Cost", typeof(double)));
            geos.Columns.Add(new DataColumn("Impressions", typeof(int)));


            for (int i = 1; i <= 10; i++)
            {
                DataRow row = geos.NewRow();
                row["Campaign"] = "Campaign" + i.ToString();
                row["Country"] = "Country" + i.ToString();
                row["Region"] = "Region" + i.ToString();
                row["MetroArea"] = "MetroArea" + i.ToString();
                row["City"] = "City" + i.ToString();
                row["Clicks"] = r.Next(100);
                row["Cost"] = r.NextDouble().ToString("N2");
                row["Impressions"] = r.Next(1, 100);

                geos.Rows.Add(row);

                totalclicks += Convert.ToInt32(row["Clicks"]);
                totalimpressions += Convert.ToInt32(row["Impressions"]);

            }

            JQGrid7.Columns.FromDataField("Clicks").FooterValue = totalclicks.ToString();
            JQGrid7.Columns.FromDataField("Impressions").FooterValue = totalimpressions.ToString();


            JQGrid7.DataSource = geos;
            JQGrid7.DataBind();


//            APIServiceProviderNamespace.main.ordersDataTable odt = DBModule.GetOrders();

        }
    }
}
