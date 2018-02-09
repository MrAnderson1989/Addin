using AddIn.Web.Routing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AddIn.Web.Razor
{
    public abstract class ActionResult
    {
        public abstract void ExecuteResult(RouteData routeData);
    }
}
