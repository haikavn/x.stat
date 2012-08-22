using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Collections;
using APIServiceProviderNamespace;

namespace fitmedia
{
    public partial class nwdgen : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            int year = DateTime.Now.Year;

            if (Request.Params["year"] != null)
            {
                try
                {
                    year = Convert.ToInt32(Request.Params["year"]);
                }
                catch
                {
                }
            }

            DateTime day = new DateTime(year, 1, 1);
            int m = 0;
            bool tr = false;

            string[] months = new string[12];
            months[0] = "Январь";
            months[1] = "Февраль";
            months[2] = "Март";
            months[3] = "Апрель";
            months[4] = "Май";
            months[5] = "Июнь";
            months[6] = "Июль";
            months[7] = "Август";
            months[8] = "Сентябрь";
            months[9] = "Октябрь";
            months[10] = "Ноябрь";
            months[11] = "Декабрь";

            ArrayList ar = new ArrayList();

            string nwdData = "<table border=\"0\" cellpadding=\"0\" cellspacing=\"1\" style=\"cursor:default; background-color:#ffffff; border:1px solid #cccccc;\" id=\"statsTable\">\r\n";

            while (day.Year == year)
            {
                if (m != day.Month)
                {
                    if (m > 0)
                    {
                        nwdData += "</tr>\r\n";

                        nwdData += "<tr>\r\n";
                        for (int i = 0; i < ar.Count; i++)
                        {
                            if (((int)ar[i]) == 0)
                                nwdData += "<td style=\"text-align: center;padding: 4px; font-size: 10px;\"><input type=\"checkbox\" id=\"" + m.ToString() + "_" + (i + 1).ToString() + "\" name=\"" + m.ToString() + "_" + (i + 1).ToString() + "\" /></td>";
                            else
                                if (((int)ar[i]) == 1)
                                    nwdData += "<td style=\"text-align: center;padding: 4px; font-size: 10px;background-color:#999999;\"><input type=\"checkbox\" checked=\"checked\" id=\"" + m.ToString() + "_" + (i + 1).ToString() + "\" name=\"" + m.ToString() + "_" + (i + 1).ToString() + "\" /></td>";
                                    else
                                        if (((int)ar[i]) == 2)
                                            nwdData += "<td style=\"text-align: center;padding: 4px; font-size: 10px;background-color:#02fe08;\"><input type=\"checkbox\" checked=\"checked\" id=\"" + m.ToString() + "_" + (i + 1).ToString() + "\" name=\"" + m.ToString() + "_" + (i + 1).ToString() + "\" /></td>";
                                            else
                                                nwdData += "<td style=\"text-align: center;padding: 4px; font-size: 10px;background-color:#999999;\"><input type=\"checkbox\" id=\"" + m.ToString() + "_" + (i + 1).ToString() + "\" name=\"" + m.ToString() + "_" + (i + 1).ToString() + "\" /></td>";
                        }
                        nwdData += "</tr>\r\n";

                        ar.Clear();
                    }

                    nwdData += "<tr>\r\n";
                    nwdData += "<td colspan=\"" + DateTime.DaysInMonth(year, day.Month).ToString() + "\">" + months[day.Month - 1] + "</td>\r\n";
                    nwdData += "</tr>\r\n";

                    nwdData += "<tr>\r\n";

                    m = day.Month;
                }

                nwdData += "<td style=\"text-align: center;padding: 4px; font-size: 10px;\">" + day.Day.ToString() + "</td>\r\n";
                bool iwd = DBModule.IsNotWorkingDay(new DateTime(day.Year, day.Month, day.Day));
                if ((day.DayOfWeek == DayOfWeek.Saturday || day.DayOfWeek == DayOfWeek.Sunday) && iwd)
                    ar.Add(1);
                else
                    if ((day.DayOfWeek != DayOfWeek.Saturday && day.DayOfWeek != DayOfWeek.Sunday) && iwd)
                        ar.Add(2);
                    else
                        if ((day.DayOfWeek == DayOfWeek.Saturday || day.DayOfWeek == DayOfWeek.Sunday) && !iwd)
                            ar.Add(3);
                            else
                                ar.Add(0);

                day = day.AddDays(1);
            }

            nwdData += "</tr>\r\n";

            nwdData += "<tr>\r\n";
            for (int i = 0; i < ar.Count; i++)
            {
                if (((int)ar[i]) == 0)
                    nwdData += "<td style=\"text-align: center;padding: 4px; font-size: 10px;\"><input type=\"checkbox\" id=\"" + m.ToString() + "_" + (i + 1).ToString() + "\" name=\"" + m.ToString() + "_" + (i + 1).ToString() + "\" /></td>";
                else
                    if (((int)ar[i]) == 1)
                        nwdData += "<td style=\"text-align: center;padding: 4px; font-size: 10px;background-color:#999999;\"><input type=\"checkbox\" checked=\"checked\" id=\"" + m.ToString() + "_" + (i + 1).ToString() + "\" name=\"" + m.ToString() + "_" + (i + 1).ToString() + "\" /></td>";
                    else
                        if (((int)ar[i]) == 2)
                            nwdData += "<td style=\"text-align: center;padding: 4px; font-size: 10px;background-color:#02fe08;\"><input type=\"checkbox\" checked=\"checked\" id=\"" + m.ToString() + "_" + (i + 1).ToString() + "\" name=\"" + m.ToString() + "_" + (i + 1).ToString() + "\" /></td>";
                        else
                            nwdData += "<td style=\"text-align: center;padding: 4px; font-size: 10px;background-color:#999999;\"><input type=\"checkbox\" id=\"" + m.ToString() + "_" + (i + 1).ToString() + "\" name=\"" + m.ToString() + "_" + (i + 1).ToString() + "\" /></td>";
            }
            nwdData += "</tr>\r\n";

            nwdData += "</table>\r\n";

            string yData = "<select id=\"year\" name=\"year\">";
            for (int i = DateTime.Now.Year - 1; i <= DateTime.Now.Year + 3; i++)
                if (year != i)
                    yData += "<option>" + i.ToString() + "</option>";
                else
                    yData += "<option selected=\"selected\">" + i.ToString() + "</option>";
            yData += "</select>";

            string path = Request.PhysicalApplicationPath;

            string html = System.IO.File.ReadAllText(path + "\\ui\\nwdedit_tpl.htm");
            html = html.Replace("{[(TBL)]}", nwdData).Replace("{[(YEAR)]}", yData).Replace("{[(LINK)]}", "&nbsp;&nbsp;<a href=\"nwdgen.aspx?year=" + year.ToString() + "\" id=\"navigate\">Показать</a>");

            System.IO.File.WriteAllText(path + "\\nwdedit.htm", html);

            Response.Redirect("nwdedit.htm");
        }
    }
}