using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using APIServiceProviderNamespace;

namespace fitmedia
{
    public partial class logview : System.Web.UI.Page
    {
        protected override void OnInit(EventArgs e)
        {
            this.EnableViewState = false;
            base.OnInit(e);
        }

        protected void refresh_Click(object sender, EventArgs e)
        {
            Response.Redirect("logview.aspx?pid=" + projects.SelectedValue + "&lt=" + logtypes.SelectedValue);
        }

        protected override void OnPreRender(EventArgs e)
        {
            int pid = 0;

            if (Request.Params["pid"] != null)
                int.TryParse(Request.Params["pid"].ToString(), out pid);

            APIServiceProviderNamespace.main.projectsDataTable pdt = DBModule.GetProjects();

            foreach (APIServiceProviderNamespace.main.projectsRow prow in pdt.Rows)
            {
                ListItem li = new ListItem(prow.name, prow.id.ToString());
                projects.Items.Add(li);
            }

            /*APIServiceProviderNamespace.main.projectsRow prow = pdt.NewprojectsRow();
            prow.id = 0;
            prow.name = "All";
            prow.pid = "All";
            prow.startfrom = DateTime.Now;
            pdt.Rows.InsertAt(prow, 0);

            projects.DataTextField = "name";
            projects.DataValueField = "id";
            projects.DataSource = pdt;
            projects.DataBind();
            projects.SelectedIndex = 0;*/

            log.DataSource = DBModule.GetErrorLogs(pid);
            log.DataBind();
            
            base.OnPreRender(e);
        }

        protected void Page_Load(object sender, EventArgs e)
        {

        }
    }
}