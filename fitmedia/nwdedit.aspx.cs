using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using APIServiceProviderNamespace;

namespace fitmedia
{
    public partial class nwdedit : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            int year = DateTime.Now.Year;
            int.TryParse(Request.Form["year"].ToString(), out year);

            string def = Request.Form["default"];

            DBModule.DeleteNotWorkingDays(year);

            for (int i = 1; i <= 12; i++)
            {
                int dim = DateTime.DaysInMonth(year, i);
                for (int j = 1; j <= dim; j++)
                {
                    DateTime dt = new DateTime(year, i, j);
                    string s = Request.Form[i.ToString() + "_" + j.ToString()];
                    if (s != null || (def != null && (dt.DayOfWeek == DayOfWeek.Saturday || dt.DayOfWeek == DayOfWeek.Sunday)))
                    {
                        DBModule.AddNotWorkingDay(dt);
                    }
                }
            }

            Response.Redirect("nwdgen.aspx?year=" + year.ToString());
        }
    }
}