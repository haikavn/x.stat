using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using APIServiceProviderNamespace;

namespace fitmedia
{
    public partial class balance : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string path = Request.PhysicalApplicationPath;

            APIServiceProviderNamespace.main.campaignsDataTable cdt = DBModule.GetCampaignsByProjectID(0);
            string provider = "";
            string client = "";

            string html = "";

            foreach (APIServiceProviderNamespace.main.campaignsRow row in cdt.Rows)
            {
                if (row.providertype != 2) continue;

                if (provider != row.provider)
                {
                    if (client.Length > 0)
                        html += "</table><br /><br />";
                    html += "<span><b>" + row.provider + "</b></span><br /><br />";
                    provider = row.provider;
                    client = "";
                }

                if (client != row.client)
                {
                    if (client.Length > 0)
                        html += "</table></ br>";
                    html += "<span><b><i>" + row.client + "</i></b></span><br />";
                    html += "<table style=\"cursor:default; background-color:#FFFFFF; border:1px solid #cccccc;\">";
                    client = row.client;
                }

                if (row.balance < 50)
                    html += "<tr style=\"color:#FF0000\">";
                else
                    html += "<tr>";

                APIServiceProviderNamespace.main.balancehistoryDataTable bdt = DBModule.GetBalanceHistory(row.id, 3);

                decimal spent = 0;

                if (bdt.Rows.Count > 0)
                {
                    APIServiceProviderNamespace.main.balancehistoryRow brow1 = (APIServiceProviderNamespace.main.balancehistoryRow)bdt.Rows[0];
                    APIServiceProviderNamespace.main.balancehistoryRow brow2 = (APIServiceProviderNamespace.main.balancehistoryRow)bdt.Rows[bdt.Rows.Count - 1];
                    spent = brow1.balance - brow2.balance;
                }

                html += "<td style=\"border:1px solid #cccccc;\">" + row.name + "</td>";
                html += "<td style=\"border:1px solid #cccccc;\">" + row.balance.ToString() + "</td>";
                html += "<td style=\"border:1px solid #cccccc;\">" + row.maxtransfer.ToString() + "</td>";
                html += "<td style=\"border:1px solid #cccccc;\">" + spent.ToString() + "</td>";
                //html += "<td><input type=\"radio\" id=\"" + "r_" + provider + "_" + client + "_" + row.id.ToString() + "\" name=\"" + provider + "_" + client + "\" value=\"" + row.id.ToString() + "\"></td>";
                //html += "<td><input type=\"text\" name=\"" + "t_" + provider + "_" + client + "_" + row.id.ToString() + "\"></td>";

                html += "</tr>";
            }

            html += "</table>";

            /*html += "";
            string select = "";
            string options = "";

            foreach (APIServiceProviderNamespace.main.campaignsRow row in cdt.Rows)
            {
                if (row.providertype != 2) continue;

                if (provider != row.provider)
                {
                    if (client.Length > 0)
                    {
                        html += "</select>&nbsp;<label>To:</label><select id=\"to\" name=\"to\">" + options + "</select><label>Amount:</label><input type=\"text\" name=\"amount\" /><br /><br /><br />";
                        options = "";
                    }
                    html += "<span><b>" + row.provider + "</b></span><br /><br />";
                    provider = row.provider;
                    client = "";
                }

                if (client != row.client)
                {
                    if (client.Length > 0)
                    {
                        html += "</select>&nbsp;<label>To:</label><select id=\"to\" name=\"to\">" + options + "</select><label>Amount:</label><input type=\"text\" name=\"amount\" /><br /><br /><br />";
                        options = "";
                    }

                    html += "<span><b><i>" + row.client + "</i></b></span><br /><hr>";
                    html += "<label>From:</label><select id=\"from\" name=\"from\">";
                    client = row.client;
                }

                html += "<option>" + row.name + " - " + row.balance.ToString() + "</option>";
                options += "<option>" + row.name + " - " + row.balance.ToString() + "</option>";
            }

            html += "</select>&nbsp;<label>To:</label><select id=\"to\" name=\"to\">" + options + "</select><label>Amount:</label><input type=\"text\" name=\"amount\" /><br /><br /><br />";*/


            string js = System.IO.File.ReadAllText(path + "\\ui\\balance_ui_tpl.js");

            js = js.Replace("{[(BAL)]}", html);

            System.IO.File.WriteAllText(path + "\\ui\\balance_ui.js", js);
        }
    }
}