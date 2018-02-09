using AddIn.Web.Routing;
using AddInWebApp.Common;
using System;
using System.Collections.Generic;
using System.Web;
namespace AddInWebApp
{
    public class Global : HttpApplication
    {
        protected void Application_Start(object sender, EventArgs e)
        {
            AddInService addInService = new AddInService();
            addInService.InitPath();
            List<AddInWebApp.Common.AddIn> addIn = addInService.GetAddIn();
            addInService.LoadAddIn(addIn);
            Dictionary<string, object> dictionary = new Dictionary<string, object>();
            dictionary.Add("addindomain", "AddInsDomain");
            dictionary.Add("controller", "AddInsService");
            dictionary.Add("action", "AddInDomains");
            dictionary.Add("id", null);
            dictionary.Add("namespaces", "AddInWebApp.Controllers");
            dictionary.Add("assembly", "AddInWebApp");
            RouteTable.Routes.Add("defaultRoute", new Route("{addindomain}/{controller}/{action}/{id}", dictionary, new AddInRouteHandler()));
        }
        protected void Session_Start(object sender, EventArgs e)
        {
        }
        protected void Application_BeginRequest(object sender, EventArgs e)
        {
        }
        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {
        }
        protected void Application_Error(object sender, EventArgs e)
        {
        }
        protected void Session_End(object sender, EventArgs e)
        {
        }
        protected void Application_End(object sender, EventArgs e)
        {
        }
    }
}
