using AddIn.Web.Routing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AddIn.Web.Controller
{
    //这个类主要定义约束
    public abstract class AddInControllerBase:IAddInController
    {

        public abstract void Execute(RouteData routeData);
    }
}
