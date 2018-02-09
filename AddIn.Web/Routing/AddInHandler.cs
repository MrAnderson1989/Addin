using AddIn.Web.Controller;
using AddIn.Web.ControllerFactory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AddIn.Web.Routing
{
    public class AddInHandler : IHttpHandler
    {
        public AddInHandler()
        { }

        public HttpContextBase Context { get; set; }
        public RouteData RouteData { get; set; }
        //通过构造函数将两个对象传过来，替代了原来RequestContext的作用
        public AddInHandler(RouteData routeData, HttpContextBase context)
        {
            RouteData = routeData;
            Context = context;
        }

        public virtual bool IsReusable
        {
            get { return false; }
        }

        public virtual void ProcessRequest(HttpContext context)
        {
            //写入MVC的版本到HttpHeader里面
            //AddVersionHeader(httpContext);
            //移除参数
            //RemoveOptionalRoutingParameters();

            //过滤文件请求
            //string url = context.Request.RawUrl;
            //if (!url.Contains("."))
            //{
            //1.从当前的RouteData里面得到请求的控制器名称
            string domainName = RouteData.RouteValue["addindomain"].ToString();
            string controllerName = RouteData.RouteValue["controller"].ToString();
            string actionName = RouteData.RouteValue["action"].ToString();

            if (domainName.ToLower() != "AddInsDomain".ToLower())
            {
                try
                {
                    if (RouteData.RouteValue.ContainsKey("postData"))
                    {
                        object[] parameters = ((List<object>)RouteData.RouteValue["postData"]).ToArray();

                        object obj = AddIn.Entity.AddInManager.Execute(domainName, controllerName, actionName, parameters);

                        context.Response.Write(obj.ToString());
                    }
                    else
                    {
                        object obj = AddIn.Entity.AddInManager.Execute(domainName, controllerName, actionName, null);
                        context.Response.Write(obj.ToString());
                    }
                }
                catch (Exception ex)
                {
                    context.Response.Write(ex.Message);
                }
                context.Response.End();

            }
            else
            {
                //2.得到控制器工厂
                IAddInControllerFactory factory = new AddInControllerFactory();

                //3.通过默认控制器工厂得到当前请求的控制器对象
                IAddInController controller = factory.CreateController(RouteData, controllerName);
                if (controller == null)
                {
                    context.Response.Write("找不到控制器！");
                    return;
                }

                try
                {
                    //4.执行控制器的Action
                    controller.Execute(RouteData);
                }
                catch (Exception ex)
                {
                    Common.Log.Write("/" + domainName + "/" + controllerName + "/" + actionName + ":" + ex.ToString());
                    context.Response.Write(ex.Message);
                }
                finally
                {
                    //5.释放当前的控制器对象
                    factory.ReleaseController(controller);
                }
            }
            //}
        }
    }
}