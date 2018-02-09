using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace AddIn.Web.Routing
{
    public interface IRouteHandler
    {
        System.Web.IHttpHandler GetHttpHandler(RouteData routeData, HttpContextBase context);
    }
}
