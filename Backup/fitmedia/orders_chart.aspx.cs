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
    public partial class orders_chart : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            APIServiceProviderNamespace.main.WeeklyOrdersCountDataTable dt = DBModule.GetWeeklyOrdersCount(DateTime.Now.Year);

            Controls.Add(new LiteralControl("<table>"));
            Controls.Add(new LiteralControl("<caption>Заказы по неделям. " + DateTime.Now.Year.ToString() + "г. </caption>"));
            Controls.Add(new LiteralControl("<thead>"));
            Controls.Add(new LiteralControl("<tr>"));
//            Controls.Add(new LiteralControl("<td></td>"));
            foreach (DataRow row in dt.Rows)
            {
                Controls.Add(new LiteralControl("<th>"));
                Controls.Add(new LiteralControl(row["w"].ToString()));
                Controls.Add(new LiteralControl("</th>"));
            }
            Controls.Add(new LiteralControl("</tr>"));
            Controls.Add(new LiteralControl("</thead>"));

            Controls.Add(new LiteralControl("<tbody>"));
            int week = 0;
            foreach (DataRow row in dt.Rows)
            {
                if (week != Convert.ToInt32(dt.Rows[0]["w"]))
                {
                    if (week > 0)
                    Controls.Add(new LiteralControl("</tr>"));
                    Controls.Add(new LiteralControl("<tr>"));
                    week = Convert.ToInt32(dt.Rows[0]["w"]);
                }
                Controls.Add(new LiteralControl("<td>"));
                Controls.Add(new LiteralControl(row["c"].ToString()));
                Controls.Add(new LiteralControl("</td>"));
            }
            Controls.Add(new LiteralControl("</tr>"));
            Controls.Add(new LiteralControl("</tbody>"));

            Controls.Add(new LiteralControl("</table>"));

        }
    }
}
