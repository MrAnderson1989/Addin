using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AddIn.Web.Routing
{
    public class RouteCollection
    {
        public Route Route { get; set; }

        public string Name { get; set; }

        //Global.asax里面配置路由规则和默认路由
        public void Add(string name, Route route)
        {
            Route = route;
            Name = name;
        }

        //通过上下文对象得到当前请求的路由表
        public RouteData GetRouteData(HttpContextBase context)
        {
            try
            {
                var RouteData = new RouteData();
                //1.配置RouteHandler实例，这里的RouteHandler是在全局配置里面写进来的
                RouteData.RouteHandler = Route.RouteHandler;

                //2.获取当前请求的虚拟路径和说明
                var virtualPath = context.Request.AppRelativeCurrentExecutionFilePath.Substring(2) + context.Request.PathInfo;

                //3.先将默认路由配置写入当前请求的路由表
                //每次请求只能读取默认值，而不能覆盖默认值
                RouteData.RouteValue = new Dictionary<string, object>();
                foreach (var key in this.Route.DefaultPath)
                {
                    RouteData.RouteValue[key.Key] = key.Value;
                }

                //4.如果当前请求虚拟路径为空，则访问默认路由表。否则从当前请求的url里面去取当前的controller和action的名称
                if (!string.IsNullOrEmpty(virtualPath))
                {
                    var arrTemplatePath = this.Route.TemplateUrl.Split("{}/".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                    var arrRealPath = virtualPath.Split("/".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                    for (var i = 0; i < arrTemplatePath.Length; i++)
                    {
                        var realPath = arrRealPath.Length > i ? arrRealPath[i] : null;
                        if (realPath == null)
                        {
                            break;
                        }
                        RouteData.RouteValue[arrTemplatePath[i]] = realPath;
                    }
                }


                //6.读取请求请求类型
                RouteData.RouteValue["HttpMethod"] = context.Request.HttpMethod;

                //7.post获取数据
                var postData = new List<object>();
                for (int i = 0; i < context.Request.Form.Count; i++)
                {
                    postData.Add(context.Request.Form[i]);
                }
                RouteData.RouteValue["postData"] = postData;

                //5.去读当前请求的参数列表
                var querystring = context.Request.QueryString.ToString();
                if (string.IsNullOrEmpty(querystring))
                {
                    return RouteData;
                }

                var parameters = querystring.Split("&".ToArray(), StringSplitOptions.RemoveEmptyEntries);

                var oparam = new Dictionary<string, string>();
                foreach (var parameter in parameters)
                {
                    var keyvalue = parameter.Split("=".ToArray());
                    oparam[keyvalue[0]] = keyvalue[1];
                }
                RouteData.RouteValue["parameters"] = oparam;

                

                return RouteData;
            }
            catch (Exception e)
            {
                AddIn.Common.Log.Write(e.ToString());
                HttpContext.Current.Response.Write(e.ToString());
                return null;
            }
        }
    }
}