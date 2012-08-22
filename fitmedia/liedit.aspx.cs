using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using APIServiceProviderNamespace;
using System.IO;

namespace fitmedia
{
    public partial class liedit : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            int pid = 0;
            int rid = 0;
            string csv = "";
            string reportname = "";


            try
            {
                int.TryParse(Request.Form["projects"].ToString(), out pid);
                int.TryParse(Request.Form["reports"].ToString(), out rid);
                csv = Request.Form["csv"].ToString();
                reportname = Request.Form["reportname"].ToString();

                if (rid == 0 && reportname.Length == 0)
                    Response.Redirect("lidataedit.aspx");

                if (rid == 0)
                    rid = DBModule.AddLIReport(reportname);

                if (rid == 0)
                    Response.Redirect("lidataedit.aspx");

                StringReader sr = new StringReader(csv);
                
                string s = sr.ReadLine();
                string[] cols = null;
                if (s != null)
                    cols = s.Split(new char[1] { ';' });
                s = sr.ReadLine();
                int r = 0;

                string[] months = new string[12] { "янв", "фев", "мар", "апр", "май", "июн", "июл", "авг", "сен", "окт", "ноя", "дек"};

                while (s != null)
                {
                    string[] row = s.Split(new char[1] { ';' });

                    if (row.Length > 0)
                    {
                        DateTime dt = DateTime.Now;
                        string dts = row[0].Replace("\"", "");
                        int m = 0;
                        int d = DateTime.Now.Day;
                        
                        for (int j = 0; j < months.Length; j++)
                            if (dts.Contains(months[j]))
                            {
                                m = j + 1;
                                dts = dts.Replace(" " + months[j], "");
                                int.TryParse(dts, out d);
                                break;
                            }

                        if (m > 0)
                        {
                            dt = new DateTime(DateTime.Now.Year, m, d, 0, 0, 0);

                            for (int i = 1; i < cols.Length; i++)
                            {
                                if (i > row.Length - 1) break;
                                DBModule.AddLIDetail(pid, rid, dt, Convert.ToInt32(row[i]), cols[i].Replace("\"", ""));
                            }
                        }
                    }
                    s = sr.ReadLine();
                }
                
                //DBModule.
            }
            catch(Exception ex)
            {
            }

            Response.Redirect("lidataedit.aspx");
        }
    }
}