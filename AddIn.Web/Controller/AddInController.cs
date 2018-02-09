﻿using AddIn.Web.Razor;
using AddIn.Web.Routing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace AddIn.Web.Controller
{
    public abstract class AddInController : AddInControllerBase, IDisposable
    {

        public override void Execute(RouteData routeData)
        {
            //1.得到当前控制器的类型
            Type type = this.GetType();

            //2.从路由表中取到当前请求的action名称
            string actionName = routeData.RouteValue["action"].ToString();

            //3.从路由表中取到当前请求的Url参数
            object parameter = null;
            
            //url中的参数
            if (routeData.RouteValue.ContainsKey("parameters"))
            {
                parameter = routeData.RouteValue["parameters"];
            }


            var paramTypes = new List<Type>();
            List<object> parameters = new List<object>();
            if (parameter != null)
            {
                var dicParam = (Dictionary<string, string>)parameter;
                foreach (var pair in dicParam)
                {
                    parameters.Add(pair.Value);
                    paramTypes.Add(pair.Value.GetType());
                }
            }

            //4.通过action名称和对应的参数反射对应方法。
            //这里第二个参数可以不理会action字符串的大小写，第四个参数决定了当前请求的action的重载参数类型
            System.Reflection.MethodInfo mi = type.GetMethod(actionName,
                BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase,null, paramTypes.ToArray(), null);
            if (mi != null)
            {
                bool flag = false;
                object[] httpMethods = mi.GetCustomAttributes(typeof(HttpMethodAttribute), true);
                if (httpMethods != null && httpMethods.Length > 0)
                {
                    foreach (var m in httpMethods)
                    {
                        if (((HttpMethodAttribute)m).HttpMethods.Contains(routeData.RouteValue["HttpMethod"]))
                        {
                            flag = true;
                        }
                    }
                }



                if (flag)
                {
                    if (mi.ReturnType.Equals(typeof(void)))
                    {
                        mi.Invoke(this, parameters.ToArray());
                    }
                    else
                    {
                        //5.执行该Action方法
                        var actionResult = mi.Invoke(this, parameters.ToArray()) as ActionResult;

                        //6.得到action方法的返回值，并执行具体ActionResult的ExecuteResult()方法。
                        actionResult.ExecuteResult(routeData);
                    }
                }
                else
                {
                    HttpResponse response = HttpContext.Current.Response;
                    response.Write("参数错误：HttpMethod");
                    response.End();
                }
            }
            else
            {
                HttpResponse response = HttpContext.Current.Response;
                response.Write("找不到Action");
                response.End();
            }
        }

        public void Dispose()
        {
            //throw new NotImplementedException();
        }

        #region View相关

        #region ContentResult
        protected virtual ContentResult Content(string content)
        {
            return Content(content, null);
        }

        protected virtual ContentResult Content(string content, string contentType)
        {
            return Content(content, contentType, null);
        }

        protected virtual ContentResult Content(string content, string contentType, Encoding contentEncoding)
        {
            return new ContentResult()
            {
                Content = content,
                ContentType = contentType,
                ContentEncoding = contentEncoding
            };
        }
        #endregion

        #region JsonResult
        protected virtual JsonResult Json(object data, JsonRequestBehavior jsonBehavior)
        {
            return new JsonResult()
            {
                Data = data,
                JsonRequestBehavior = jsonBehavior
            };
        }
        #endregion

        #region ViewResult
        protected virtual MyViewResult MyView()
        {
            return new MyViewResult();
        }

        protected virtual MyViewResult MyView(object data)
        {
            return new MyViewResult()
            {
                Data = data
            };
        }

        protected virtual RazorEngineViewResult RazorEngineView()
        {
            return new RazorEngineViewResult();
        }

        protected virtual RazorEngineViewResult RazorEngineView(object data)
        {
            return new RazorEngineViewResult()
            {
                Data = data
            };
        }

        protected virtual VelocityViewResult VelocityView()
        {
            return new VelocityViewResult();
        }

        protected virtual VelocityViewResult VelocityView(object data)
        {
            return new VelocityViewResult()
            {
                Data = data
            };
        }
        #endregion

        #endregion
    }
}
