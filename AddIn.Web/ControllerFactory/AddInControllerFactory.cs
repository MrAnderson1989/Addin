using AddIn.Web.Controller;
using AddIn.Web.Routing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace AddIn.Web.ControllerFactory
{
    public class AddInControllerFactory:IAddInControllerFactory
    {
        #region Public
        //通过当前的请求上下文和控制器名称得到控制器的对象
        public IAddInController CreateController(RouteData routeData, string controllerName)
        {
            if (routeData == null)
            {
                throw new ArgumentNullException("routeData");
            }

            if (string.IsNullOrEmpty(controllerName))
            {
                throw new ArgumentException("controllerName");
            }
            //得到当前的控制类型
            Type controllerType = GetControllerType(routeData, controllerName);
            if (controllerType == null)
            {
                return null;
            }
            //得到控制器对象
            IAddInController controller = GetControllerInstance(routeData, controllerType);
            return controller;
        }
            
        //释放控制器对象
        public void ReleaseController(IAddInController controller)
        {
            IDisposable disposable = controller as IDisposable;
            if (disposable != null)
            {
                disposable.Dispose();
            }
        }
        #endregion

        #region Privates
        //得到当前请求的控制器实例
        private IAddInController GetControllerInstance(RouteData routeData, Type controllerType)
        {
            var oRes = Activator.CreateInstance(controllerType) as IAddInController;
            return oRes;
        }

        //得到当前请求的控制器类型
        private Type GetControllerType(RouteData routeData, string controllerName)
        {
            //从路由配置信息里面读取命名空间和程序集
            object routeNamespaces = routeData.RouteValue["namespaces"];
            object routeAssembly = routeData.RouteValue["assembly"];
            //requestContext.RouteData.Values.TryGetValue("namespaces", out routeNamespaces);
            //requestContext.RouteData.Values.TryGetValue("assembly", out routeAssembly);

            //通过反射得到控制器的类型
            var type = Assembly.Load(routeAssembly.ToString()).GetType(routeNamespaces.ToString() + "." + controllerName + "Controller", false, true);

            return type;
        }
        #endregion
    }
}
