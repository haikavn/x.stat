using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using APIServiceProviderNamespace;

namespace fitmedia
{
    public partial class lidataedit : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            DateTime sd = DateTime.Now, ed = DateTime.Now;


            string js = "";

            APIServiceProviderNamespace.main.projectsDataTable pdt = DBModule.GetProjects();

            foreach (APIServiceProviderNamespace.main.projectsRow pr in pdt.Rows)
            {
                js += "$('<option />').attr('value', '" + pr.id.ToString() + "').text('" + pr.name + "').appendTo('#projects');";
            }

            js += "$('<option />').attr('value', '0').text('Other').appendTo('#reports');";

            APIServiceProviderNamespace.main.li_reportsDataTable rdt = DBModule.GetLIReports();

            foreach (APIServiceProviderNamespace.main.li_reportsRow rr in rdt.Rows)
            {
                js += "$('<option />').attr('value', '" + rr.id.ToString() + "').text('" + rr.name + "').appendTo('#reports');";
            }

            string path = Request.PhysicalApplicationPath;
            System.IO.File.WriteAllText(path + "\\ui\\ui_liedit.js", js);

            Response.Redirect("liedit.htm");
        }
    }
}