using AddIn.Web.Controller;
using AddIn.Web.Routing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AddIn.Web.ControllerFactory
{
    //控制器创建工厂
    public interface IAddInControllerFactory
    {
        //创建控制器
        IAddInController CreateController(RouteData routeData, string controllerName);
        
        //释放控制器
        void ReleaseController(IAddInController controller);
    }
}
