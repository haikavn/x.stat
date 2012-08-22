using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using APIServiceProviderNamespace;

namespace fitmedia
{
    public partial class dataedit : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            DateTime sd = DateTime.Now, ed = DateTime.Now;

            try
            {
                sd = DateTime.ParseExact(Request.Params["sd"].ToString(), "dd.MM.yyyy", null);
            }
            catch
            {
                sd = DateTime.Now;
            }

            try
            {
                ed = DateTime.ParseExact(Request.Params["ed"].ToString(), "dd.MM.yyyy", null);
            }
            catch
            {
                ed = DateTime.Now;
            }

            sd = new DateTime(sd.Year, sd.Month, sd.Day, 0, 0, 0);
            ed = new DateTime(ed.Year, ed.Month, ed.Day, 23, 59, 59);

            string js = "$('#startdate').val('" + sd.ToString("dd.MM.yyyy") + "');$('#enddate').val('" + ed.ToString("dd.MM.yyyy") + "');$('<option />').attr('value', '0').text('All').appendTo('#projects');";


            int pid = 0;

            if (Request.Params["pid"] != null)
                int.TryParse(Request.Params["pid"].ToString(), out pid);

            APIServiceProviderNamespace.main.projectsDataTable pdt = DBModule.GetProjects();

            foreach (APIServiceProviderNamespace.main.projectsRow pr in pdt.Rows)
            {
                js += "$('<option />').attr('value', '" + pr.id.ToString() + "').text('" + pr.name + "').appendTo('#projects1');";
                js += "$('<option />').attr('value', '" + pr.id.ToString() + "').text('" + pr.name + "').appendTo('#projects2');";
                if (pid == 0 || (pid > 0 && pid != pr.id))
                    js += "$('<option />').attr('value', '" + pr.id.ToString() + "').text('" + pr.name + "').appendTo('#projects');";
                else
                {
                    // if (pid == pr.id)
                    js += "$('<option />').attr('value', '" + pr.id.ToString() + "').attr('selected', 'selected').text('" + pr.name + "').appendTo('#projects');";
                }
            }


            APIServiceProviderNamespace.main.GetCustomExpensesDataTable dt = DBModule.GetCustomExpenses(sd, ed, pid);

            string xmldata = "<rows>\n";

            foreach (APIServiceProviderNamespace.main.GetCustomExpensesRow row in dt.Rows)
            {
                xmldata += "<row id='" + row.id.ToString() + "'>\n";
                xmldata += "<cell><![CDATA[" + row.dt.ToString("dd.MM.yyyy") + "]]></cell>\n";
                xmldata += "<cell><![CDATA[" + row.pname + "]]></cell>\n";
                xmldata += "<cell>" + row.clicks.ToString() + "</cell>\n";
                xmldata += "<cell>" + row.shows.ToString() + "</cell>\n";
                //xmldata += "<cell>" + row.price.ToString("### ### ###") + " р." + "</cell>\n";
                xmldata += "<cell>" + row.price.ToString("0.00") + "</cell>\n";
                xmldata += "<cell>" + row.expenses.ToString("0.00") + "</cell>\n";
                xmldata += "<cell>" + row.projectid.ToString() + "</cell>\n";
                xmldata += "</row>\n";
            }

            xmldata += "</rows>\n";

            js += "$('#navigate').attr('href', 'dataedit.aspx?sd=' + $('#startdate').val() + '&ed=' + $('#enddate').val() + '&pid=' + $('#projects').val());";
            js += "$('#projectid').attr('value', $('#projects').val());";
            js += "$('#back').attr('href', 'datagen.aspx?sd=' + $('#startdate').val() + '&ed=' + $('#enddate').val() + '&pid=' + $('#projects').val());";

            string path = Request.PhysicalApplicationPath;
            System.IO.File.WriteAllText(path + "\\ui\\grideditdata.xml", xmldata);
            System.IO.File.WriteAllText(path + "\\ui\\ui_edit.js", js);

            Response.Redirect("edit.htm");
        }
    }
}