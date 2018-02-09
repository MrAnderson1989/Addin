using AddIn.Common;
using AddIn.Entity;
using AddIn.Interface;
using AddIn.Web.Controller;
using AddIn.Web.Razor;
using AddIn.Web.Routing;
using AddInWebApp.Common;
using Microsoft.CSharp.RuntimeBinder;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Dynamic;
using System.Runtime.CompilerServices;
using System.Web;
namespace AddInWebApp.Controllers
{
    public class AddInsServiceController : AddIn.Web.Controller.AddInController
    {
        [HttpGet]
        public ActionResult AddInDomains()
        {
            HttpRequest request = HttpContext.Current.Request;
            if (request.HttpMethod == "GET")
            {
                List<object> list = new List<object>();
                SqlHelper sqlHelper = new SqlHelper();
                DataTable dataTable = sqlHelper.ExecuteDataTable("select * from T_AddIn ", new SqlParameter[0]);
                if (dataTable != null && dataTable.Rows.Count > 0)
                {
                    foreach (DataRow dataRow in dataTable.Rows)
                    {
                        var item = new
                        {
                            AddInDomain = dataRow["F_AppDomain_Name"].ToString(),
                            Description = dataRow["F_Description"].ToString(),
                            ID = dataRow["F_ID"].ToString()
                        };
                        list.Add(item);
                    }
                }
                return RazorEngineView(new
                {
                    Models = list
                });
            }
            return Json(new
            {
                errcode = 405,
                errmsg = "Method Not Allowed",
                msgdoc = string.Concat(new object[]
                {
                    "http://",
                    request.Url.Host,
                    ":",
                    request.Url.Port,
                    "/AddInsDomain/help/doc"
                })
            }, JsonRequestBehavior.DenyGet);
        }

        [HttpGet]
        public ActionResult AddInDomain(string ID)
        {
            HttpRequest request = HttpContext.Current.Request;
            ActionResult result;
            try
            {
                SqlHelper sqlHelper = new SqlHelper();
                DataTable dataTable = sqlHelper.ExecuteDataTable("select * from T_AddIn where f_id = " + ID, new SqlParameter[0]);
                if (dataTable != null && dataTable.Rows.Count > 0)
                {
                    string appDomainName = dataTable.Rows[0]["F_AppDomain_Name"].ToString();
                    string description = dataTable.Rows[0]["F_Description"].ToString();
                    string status = dataTable.Rows[0]["F_Status"].ToString();
                    List<object> list = new List<object>();
                    if (!AddInManager.AppDomains.ContainsKey(appDomainName))
                    {
                        result = this.RazorEngineView(new
                        {
                            ID = ID,
                            AppDomainName = appDomainName,
                            Description = description,
                            Status = status,
                            Controllers = list
                        });
                    }
                    else
                    {
                        if (AddInManager.Loaders[appDomainName] == null)
                        {
                            AddInManager.CreateAddInAppDomain(appDomainName);
                            AddIn.Common.Log.Write("创建新的代理类。");
                        }
                        try
                        {
                            foreach (IController current in AddInManager.Loaders[appDomainName].GetControllers())
                            {
                                List<object> list2 = new List<object>();
                                foreach (IAction current2 in current.Actions)
                                {
                                    List<object> list3 = new List<object>();
                                    foreach (IParameter current3 in current2.Parameters)
                                    {
                                        var item = new
                                        {
                                            ParameterName = current3.ParameterName,
                                            ParameterType = current3.ParameterType.Name
                                        };
                                        list3.Add(item);
                                    }
                                    var item2 = new
                                    {
                                        ActionName = current2.ActionName,
                                        ReturnType = current2.ReturnType.Name,
                                        HttpMethod = current2.HttpMethod,
                                        Description = current2.Description,
                                        Parameters = list3
                                    };
                                    list2.Add(item2);
                                }
                                var item3 = new
                                {
                                    ControllerName = current.ControllerName,
                                    Actions = list2
                                };
                                list.Add(item3);
                            }
                        }
                        catch (Exception ex)
                        {
                            AddInWebApp.Common.Log.log(ex.ToString());
                        }
                        result = this.RazorEngineView(new
                        {
                            ID = ID,
                            AppDomainName = appDomainName,
                            Description = description,
                            Status = status,
                            Controllers = list
                        });
                    }
                }
                else
                {
                    result = this.Json(new
                    {
                        errcode = 0,
                        errmsg = "OK",
                        msgdoc = string.Concat(new object[]
                        {
                            "http://",
                            request.Url.Host,
                            ":",
                            request.Url.Port,
                            "/AddInsDomain/help/doc"
                        })
                    }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex2)
            {
                result = this.Json(new
                {
                    errcode = -1,
                    errmsg = ex2.Message,
                    msgdoc = string.Concat(new object[]
                    {
                        "http://",
                        request.Url.Host,
                        ":",
                        request.Url.Port,
                        "/AddInsDomain/help/doc"
                    })
                }, JsonRequestBehavior.AllowGet);
            }
            return result;
        }
    }
}
