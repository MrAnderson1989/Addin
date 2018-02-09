using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AddIn.Web.Controller;
using AddIn.Entity;
using AddIn.Web.Razor;
using System.Dynamic;

namespace AddInWebApp.Controllers
{
    public class HelpController : AddIn.Web.Controller.AddInController
    {
        //public void AddIns()
        //{
        //    if (HttpContext.Current.Request.HttpMethod == "GET")
        //    {
        //        string html = "<ul>";
        //        foreach (var domain in AddInManager.AppDomains)
        //        {
        //            html += "<li><span>" + domain.Key + "</span><ul>";

        //            foreach (var c in AddInManager.Loaders[domain.Key].Controllers)
        //            {
        //                html += "<li><span>" + c.ControllerName + "</span><ul>";
        //                foreach (var a in c.Actions)
        //                {
        //                    html += "<li><span>" + a.ActionName + "():" + a.ReturnType.Name + "</span><ul>";

        //                    foreach (var p in a.Parameters)
        //                    {
        //                        html += "<li>" + p.ParameterName + ":" + p.ParameterType.Name + "</li>";
        //                    }
        //                    html += "</ul></li>";
        //                }
        //                html += "</ul></li>";
        //            }
        //            html += "</ul></li>";
        //        }
        //        html += "</ul>";
        //        HttpContext.Current.Response.Write(html);
        //    }

        //}

        public ActionResult AddIns()
        {

            //if (HttpContext.Current.Request.HttpMethod == "GET")
            //{

            //    List<dynamic> models = new List<dynamic>();
            //    foreach (var domain in AddInManager.AppDomains)
            //    {
            //        //初始化动态类型
            //        dynamic model = new ExpandoObject();
            //        model.AddInDomain = domain.Key;

            //        //controllers 集合变量
            //        List<dynamic> controllers = new List<dynamic>();
            //        foreach (var c in AddInManager.Loaders[domain.Key].Controllers)
            //        {
            //            dynamic controller = new ExpandoObject();
            //            controller.ControllerName = c.ControllerName;
            //            List<dynamic> actions = new List<dynamic>();
            //            foreach (var a in c.Actions)
            //            {
            //                dynamic action = new ExpandoObject();
            //                action.ActionName = a.ActionName;
            //                action.ReturnType = a.ReturnType.Name;

            //                List<dynamic> parameters = new List<dynamic>();
            //                foreach (var p in a.Parameters)
            //                {
            //                    dynamic parameter = new ExpandoObject();
            //                    parameter.ParameterName = p.ParameterName;
            //                    parameter.ParameterType = p.ParameterType.Name;
            //                    parameters.Add(parameter);
            //                }
            //                action.Parameters = parameters;
            //                actions.Add(action);
            //            }
            //            controller.Actions = actions;
            //            controllers.Add(controller);
            //        }

            //        model.Controllers = controllers;
            //        models.Add(model);
            //    }
            //    //return VelocityView(models);
            //    return Json(models, JsonRequestBehavior.AllowGet);
            //}

            if (HttpContext.Current.Request.HttpMethod == "GET")
            {

                List<dynamic> models = new List<dynamic>();

                foreach (var domain in AddInManager.AppDomains)
                {
                    //初始化动态类型
                    dynamic model = new ExpandoObject();
                    model.AddInDomain = domain.Key;
                    //controllers 集合变量
                    List<dynamic> controllers = new List<dynamic>();
                    var _controllers = new { };

                    // 当Loader被回收 ，创建新的代理类。
                    if (AddInManager.Loaders[domain.Key] == null)
                    {
                        AddInManager.CreateAddInAppDomain(domain.Key);
                        AddIn.Common.Log.Write("创建新的代理类。");
                    }
                    try
                    {
                        foreach (var c in AddInManager.Loaders[domain.Key].GetControllers())
                        {
                            List<dynamic> actions = new List<dynamic>();
                            foreach (var a in c.Actions)
                            {

                                List<dynamic> parameters = new List<dynamic>();
                                foreach (var p in a.Parameters)
                                {
                                    var _parameter = new { ParameterName = p.ParameterName, ParameterType = p.ParameterType.Name };
                                    parameters.Add(_parameter);
                                }
                                var _action = new { ActionName = a.ActionName, ReturnType = a.ReturnType.Name, HttpMethod = a.HttpMethod, Description = a.Description, Parameters = parameters };
                                actions.Add(_action);
                            }
                            var _controller = new { ControllerName = c.ControllerName, Actions = actions };
                            controllers.Add(_controller);
                        }
                    }
                    catch (Exception e)
                    {
                        Common.Log.log(e.ToString());
                        //return VelocityView(e.ToString());
                        return VelocityView(models);
                    }
                    var _model = new { AddInDomain = domain.Key, Controllers = controllers };
                    models.Add(_model);
                }

                return VelocityView(models);
                //return Json(models, JsonRequestBehavior.AllowGet);
            }
            return null;

        }

        public void AddIns(string ID)
        {
            if (HttpContext.Current.Request.HttpMethod == "POST")
            {
                AddInManager.CreateAddInAppDomain(ID);
                AddInManager.LoadAddInDll(ID, "");
            }
        }

        public void AddAddIn(string ID, string Path)
        {
            try
            {
                Path = "G:\\Project\\NET\\DynamicDllLoad\\Plug-inDll\\bin\\Debug\\Plug-inDll.dll";
                AddInManager.CreateAddInAppDomain(ID);
                AddInManager.LoadAddInDll(ID, Path);
                HttpContext.Current.Response.Write("加载成功！");
            }
            catch (Exception ex)
            {
                HttpContext.Current.Response.Write(ex.Message);
            }
        }

        public void Unload(string ID)
        {
            try
            {
                if (AddInManager.AppDomains.ContainsKey(ID))
                {
                    AddInManager.UnloadAddIn(ID);
                    AddInManager.AppDomains.Remove(ID);
                    if (AddInManager.Loaders.ContainsKey(ID))
                    {
                        AddInManager.Loaders.Remove(ID);
                    }
                    HttpContext.Current.Response.Write("插件域 " + ID + " 卸载成功！");
                }
                else
                {
                    HttpContext.Current.Response.Write("插件域 " + ID + " 并未加载！");
                }
            }
            catch (Exception ex)
            {
                HttpContext.Current.Response.Write(ex.Message);
            }
        }

        public void Reload()
        {
            try
            {
                Common.AddInService addInService = new Common.AddInService();
                addInService.InitPath();
                List<Common.AddIn> addins = addInService.GetAddIn();
                addInService.LoadAddIn(addins);
                HttpContext.Current.Response.Write("重新加载成功！");
            }
            catch (Exception ex)
            {
                HttpContext.Current.Response.Write(ex.Message);
            }
        }

        public ActionResult RazorTest()
        {
            //return RazorEngineView(new { Name = "小明", Age = 16, School = "育才高中" });
            return RazorEngineView(new { Name = "小明" });

        }

    }
}