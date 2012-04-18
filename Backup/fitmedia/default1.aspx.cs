using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using APIServiceProviderNamespace;
using System.IO;
using System.Reflection;
using System.Net;

namespace fitmedia
{
    public partial class default1 : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            SyncManager sm = new SyncManager();
            sm.Start();
            return;
            string path = Request.PhysicalApplicationPath;

            // if (!ClientScript.IsStartupScriptRegistered("JSScript"))
            {
                APIServiceProviderNamespace.main.projectsDataTable dt = DBModule.GetProjects();

                string treejs = "";
                treejs += "var treeItems, rootItem, rootItemElement;\r\n";

                int j = 0;

                foreach (APIServiceProviderNamespace.main.projectsRow pr in dt.Rows)
                {
                    treejs += "$('#jqxTree').jqxTree('addTo', { label: '" + pr.name + "', id:'" + pr.id.ToString() + "' });\r\n";
                    treejs += "treeItems = $(\"#jqxTree\").jqxTree('getItems');\r\n";
                    treejs += "rootItem = treeItems[" + j.ToString() + "];\r\n";
                    treejs += "rootItemElement = rootItem.element;\r\n";

                    APIServiceProviderNamespace.main.campaignsDataTable cdt = DBModule.GetCampaignsByProjectID(pr.id);

                    foreach (APIServiceProviderNamespace.main.campaignsRow cr in cdt.Rows)
                    {
                        treejs += "$('#jqxTree').jqxTree('addTo', { label: '" + cr.name + "', id:'" + cr.id.ToString() + "' }, rootItemElement);\r\n\r\n";
                        j++;
                    }

                    j++;
                }

                //treejs += "</script>";
                string js = System.IO.File.ReadAllText(path + "\\ui\\ui_tpl.js");
                js = js.Replace("{[(TREE)]}", treejs);
                System.IO.File.WriteAllText(path + "\\ui\\ui.js", js);

                //ClientScript.RegisterStartupScript(this.GetType(), "JSScript", js);
            }

            //Response.Redirect("index.html");
        }
    }
}
