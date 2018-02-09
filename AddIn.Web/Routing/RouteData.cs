using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AddIn.Web.Routing
{
    public class RouteData
    {
        public IRouteHandler RouteHandler { get; set; }

        public Dictionary<string, object> RouteValue { get; set; }
    }
}