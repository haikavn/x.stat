using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using APIServiceProviderNamespace;

namespace fitmedia
{
    public partial class orders : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            APIServiceProviderNamespace.main.ordersDataTable dt = DBModule.GetOrders();

            Controls.Add(new LiteralControl("<table cellpadding=\"0\" cellspacing=\"0\" border=\"0\" class=\"display\" id=\"example\" width=\"100%\">"));
            Controls.Add(new LiteralControl("<thead>"));
            Controls.Add(new LiteralControl("<tr>"));
            foreach (DataColumn col in dt.Columns)
            {
                Controls.Add(new LiteralControl("<th>"));
                Controls.Add(new LiteralControl(col.ColumnName));
                Controls.Add(new LiteralControl("</th>"));
            }
            Controls.Add(new LiteralControl("</tr>"));
            Controls.Add(new LiteralControl("</thead>"));

            Controls.Add(new LiteralControl("<tbody>"));
            foreach (DataRow row in dt.Rows)
            {
                Controls.Add(new LiteralControl("<tr>"));
                for (int i = 0; i < dt.Columns.Count; i++)
                {
                    Controls.Add(new LiteralControl("<td>"));
                    Controls.Add(new LiteralControl(row[i].ToString()));
                    Controls.Add(new LiteralControl("</td>"));
                }
                Controls.Add(new LiteralControl("</tr>"));
            }
            Controls.Add(new LiteralControl("</tbody>"));
            Controls.Add(new LiteralControl("<tfoot>"));
            Controls.Add(new LiteralControl("<tr>"));
            foreach (DataColumn col in dt.Columns)
            {
                Controls.Add(new LiteralControl("<th>"));
                Controls.Add(new LiteralControl(col.ColumnName));
                Controls.Add(new LiteralControl("</th>"));
            }
            Controls.Add(new LiteralControl("</tr>"));
            Controls.Add(new LiteralControl("</tfoot>"));

            Controls.Add(new LiteralControl("</table>"));
        }
    }
}
