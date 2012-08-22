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
using System.Xml;


namespace fitmedia
{
    public partial class default1 : System.Web.UI.Page
    {
       /* private static IAuthorizationState GetAuthorization(NativeApplicationClient arg)
        {
            // Get the auth URL:
            IAuthorizationState state = new AuthorizationState(new[] { AnalyticsService.Scopes.AnalyticsReadonly.ToString() });
            state.Callback = new Uri(NativeApplicationClient.OutOfBandCallbackUrl);
            Uri authUri = arg.RequestUserAuthorization(state);

            // Request authorization from the user (by opening a browser window):
            Process.Start(authUri.ToString());
            Console.Write("  Authorization Code: ");
            string authCode = Console.ReadLine();
            Console.WriteLine();

            // Retrieve the access token by using the authorization code:
            return arg.ProcessUserAuthorization(authCode, state);
        }*/

        protected void Page_Load(object sender, EventArgs e)
        {
            /*var provider = new NativeApplicationClient(GoogleAuthenticationServer.Description);
            provider.ClientIdentifier = "753811298394-qn5uqbhjk83tq5osjqnp7jmdu3b1eggf.apps.googleusercontent.com";
            provider.ClientSecret = "-2hRIom9w9g_kuRLeNLTw26n";
            var auth = new OAuth2Authenticator<NativeApplicationClient>(provider, GetAuthorization);

            AnalyticsService aservice = new AnalyticsService(auth);

            //aservice.Key = "AIzaSyC1O0Dq8KVBJEaOTe2HFV9940_xQfinN3Y";
            //aservice.CreateRequest("sss", "ddd");
            Accounts res = aservice.Management.Accounts.List().Fetch();

            return;*/

            /*string s = "<?xml version=\"1.0\" encoding=\"UTF-8\" ?><aaa>&D</aaa>";
            s = HttpUtility.UrlEncode(s);
            XmlDocument xml = new XmlDocument();
            xml.LoadXml(s);*/

            //XmlDocument xml = new XmlDocument();
            //XmlNode nd = xml.CreateNode(XmlNodeType.Text, "hhh", null);


            DateTime d = new DateTime(2012, 7, 30, 1, 59, 59);
            DateTime d1 = new DateTime(2012, 7, 30, 2, 59, 58);
            TimeSpan ts = d1 - d;

            TimeSpan ts1 = new TimeSpan(590000000);

            int aa = 01;

            bool b = (0.99 > 0.991);

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
