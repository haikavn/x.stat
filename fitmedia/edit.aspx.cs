using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using APIServiceProviderNamespace;
using System.Xml;

namespace fitmedia
{
    public partial class edit : System.Web.UI.Page
    {
        protected string GetRealString(string s)
        {
            string ss = "";
            for (int i = 0; i < s.Length; i++)
                if ((s[i] >= '0' && s[i] <= '9') || s[i] == '.')
                    ss += s[i];

            return ss;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            int id = 0;
            int projectid = 0;
            int campaignid = 0;
            DateTime dt = DateTime.Now;
            int clicks = 0;
            int shows = 0;
            decimal price = 0;
            decimal expenses = 0;

            int pid = 0;
            DateTime sd = DateTime.Now;
            DateTime ed = DateTime.Now;

         

            try
            {
                if (Request.Params["id"] != null)
                {
                    int.TryParse(Request.Form["projects1"].ToString(), out projectid);
                    int.TryParse(Request.Form["campaignid"].ToString(), out campaignid);

                    if (int.TryParse(Request.Params["id"].ToString(), out id))
                    {
                        if (id < 0)
                        {
                            DBModule.UpdateCustomExpenses(id, projectid, 0, dt, clicks, shows, price, expenses);
                            Response.Redirect("dataedit.aspx?pid=" + pid.ToString() + "&sd=" + sd.ToString("dd.MM.yyyy") + "&ed=" + ed.ToString("dd.MM.yyyy"));
                        }
                    }
                    id = 0;
                    int.TryParse(Request.Form["id"].ToString(), out id);
                    dt = DateTime.ParseExact(Request.Form["date"].ToString(), "dd.MM.yyyy", null);
                    int.TryParse(Request.Form["clicks"].ToString(), out clicks);
                    int.TryParse(Request.Form["shows"].ToString(), out shows);
                    decimal.TryParse(Request.Form["price"].ToString(), out price);
                    decimal.TryParse(Request.Form["expenses"].ToString(), out expenses);

                    sd = DateTime.ParseExact(Request.Form["startdate"].ToString(), "dd.MM.yyyy", null);
                    ed = DateTime.ParseExact(Request.Form["enddate"].ToString(), "dd.MM.yyyy", null);
                    int.TryParse(Request.Form["projects"].ToString(), out pid);

                    DBModule.UpdateCustomExpenses(id, projectid, 0, dt, clicks, shows, price, expenses);
                    Response.Redirect("dataedit.aspx?pid=" + pid.ToString() + "&sd=" + sd.ToString("dd.MM.yyyy") + "&ed=" + ed.ToString("dd.MM.yyyy"));

                }
                else
                {
                    if (Request.Form["xmldata"] != null)
                    {
                        int.TryParse(Request.Form["projects2"].ToString(), out projectid);

                        string xml = Request.Form["xmldata"].ToString();

                        try
                        {
                            XmlDocument xmldoc = new XmlDocument();
                            xmldoc.LoadXml(xml);

                            XmlElement root = xmldoc["report"];

                            if (root == null)
                                Response.Redirect("dataedit.aspx?pid=" + pid.ToString() + "&sd=" + sd.ToString("dd.MM.yyyy") + "&ed=" + ed.ToString("dd.MM.yyyy"));

                            XmlElement table = root["table"];

                            if (table == null)
                                Response.Redirect("dataedit.aspx?pid=" + pid.ToString() + "&sd=" + sd.ToString("dd.MM.yyyy") + "&ed=" + ed.ToString("dd.MM.yyyy"));

                            foreach (XmlNode node in table.ChildNodes)
                            {
                                if (node.Name != "row") continue;

                                XmlAttribute campaign = node.Attributes["campaign"];

                                if (campaign == null) continue;

                                int cid = DBModule.AddCompaign(campaign.Value, "", projectid, "adwords.google.com", projectid.ToString(), APIServiceProviderTypes.GoogleAdwords, 0, 0);

                                DateTime datetime = DateTime.ParseExact(node.Attributes["day"].Value, "yyyy-MM-dd", null);

                                int.TryParse(GetRealString(node.Attributes["clicks"].Value), out clicks);
                                
                                int.TryParse(GetRealString(node.Attributes["impressions"].Value), out shows);
                                decimal.TryParse(GetRealString(node.Attributes["avgCPC"].Value.Replace(",", ".")), out price);

                                decimal.TryParse(GetRealString(node.Attributes["cost"].Value.Replace(",", ".")), out expenses);

                                DBModule.UpdateCustomExpenses(0, projectid, cid, datetime, clicks, shows, price, expenses);
   
                            }

                            Response.Redirect("dataedit.aspx?pid=" + pid.ToString() + "&sd=" + sd.ToString("dd.MM.yyyy") + "&ed=" + ed.ToString("dd.MM.yyyy"));
                        }
                        catch
                        {
                        }

                        
                    }
                }
            }
            catch(Exception ex)
            {
            }

            
            Response.Redirect("dataedit.aspx");

        }
    } 
}