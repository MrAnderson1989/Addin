using AddIn.Web.Routing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AddIn.Web.Controller
{
    public interface IAddInController
    {
        void Execute(RouteData routeData);
    }
}
