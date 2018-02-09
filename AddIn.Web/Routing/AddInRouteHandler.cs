using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AddIn.Web.Routing
{
    public class AddInRouteHandler: IRouteHandler
    {
        /// <summary>
        /// 返回处理当前请求的HttpHandler对象
        /// </summary>
        /// <param name="routeData">当前的请求的路由对象</param>
        /// <param name="context">当前请求的下文对象</param>
        /// <returns>处理请求的HttpHandler对象</returns>
        public System.Web.IHttpHandler GetHttpHandler(RouteData routeData, HttpContextBase context)
        {
            return new AddInHandler(routeData, context);
        }
    }
}