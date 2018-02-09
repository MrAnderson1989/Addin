using AddIn.Entity;
using AddIn.Web.Controller;
using AddIn.Web.Razor;
using AddIn.Web.Routing;
using AddInWebApp.Common;
using Microsoft.CSharp.RuntimeBinder;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Runtime.CompilerServices;
using System.Web;
namespace AddInWebApp.Controllers
{
    public class AddInsApiController : AddIn.Web.Controller.AddInController
    {
        [HttpPost]
        public ActionResult Append()
        {
            HttpRequest request = HttpContext.Current.Request;
            ActionResult result;
            try
            {
                SqlHelper sqlHelper = new SqlHelper();
                string value = request.Form["Params"];

                dynamic param = JsonConvert.DeserializeObject(value);
                string appDomainName = param.Name;
                string description = param.Dscriptinon;

                DataTable dataTable = sqlHelper.ExecuteDataTable("select * from T_AddIn where F_AppDomain_Name='" + appDomainName + "'", new SqlParameter[0]);
                if (dataTable != null && dataTable.Rows.Count > 0)
                {
                    result = this.Json(new
                    {
                        errcode = 40001,
                        errmsg = "域名已被占用！",
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
                else
                {
                    int num = sqlHelper.ExecuteNonQuery(string.Concat(new string[]
                    {
                        "insert into T_AddIn values ('",
                        appDomainName,
                        "','',getdate(),-1,'",
                        description,
                        "')"
                    }), new SqlParameter[0]);
                    if (num == 1)
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
                        }, JsonRequestBehavior.DenyGet);
                    }
                    else
                    {
                        result = this.Json(new
                        {
                            errcode = 40002,
                            errmsg = "插入失败！",
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
                }
            }
            catch (Exception ex)
            {
                Log.log(ex.ToString());
                result = this.Json(new
                {
                    errcode = -1,
                    errmsg = ex.Message,
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
            return result;
        }
        [HttpPost]
        public ActionResult Unload()
        {
            HttpRequest request = HttpContext.Current.Request;
            ActionResult result;
            try
            {
                string value = request.Form["Params"];
                dynamic param = JsonConvert.DeserializeObject(value);

                string appDomainID = param.AppDomainID;
                SqlHelper sqlHelper = new SqlHelper();
                DataTable dataTable = sqlHelper.ExecuteDataTable("select * from T_AddIn where F_ID =" + appDomainID, new SqlParameter[0]);
                string appDomianName = (dataTable != null && dataTable.Rows.Count > 0) ? dataTable.Rows[0]["F_AppDomain_Name"].ToString() : "";
                if (appDomianName != "")
                {
                    if (AddInManager.AppDomains.ContainsKey(appDomianName))
                    {
                        AddInManager.UnloadAddIn(appDomianName);
                        AddInManager.AppDomains.Remove(appDomianName);
                        if (AddInManager.Loaders.ContainsKey(appDomianName))
                        {
                            AddInManager.Loaders.Remove(appDomianName);
                        }
                        sqlHelper.ExecuteNonQuery("update T_AddIn set F_Status = 0 where F_ID=" + appDomainID, new SqlParameter[0]);
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
                        }, JsonRequestBehavior.DenyGet);
                    }
                    else
                    {
                        result = this.Json(new
                        {
                            errcode = 40003,
                            errmsg = "尝试卸载未加载的插件域！",
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
                }
                else
                {
                    result = this.Json(new
                    {
                        errcode = 40004,
                        errmsg = "参数错误：ID！",
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
            }
            catch (Exception ex)
            {
                Log.log(ex.ToString());
                result = this.Json(new
                {
                    errcode = -1,
                    errmsg = ex.Message,
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
            return result;
        }
        [HttpPost]
        public ActionResult ReloadAll()
        {
            HttpRequest request = HttpContext.Current.Request;
            ActionResult result;
            try
            {
                AddInService addInService = new AddInService();
                addInService.InitPath();
                List<Common.AddIn> addIn = addInService.GetAddIn();
                addInService.LoadAddIn(addIn);
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
                }, JsonRequestBehavior.DenyGet);
            }
            catch (Exception ex)
            {
                Log.log(ex.ToString());
                result = this.Json(new
                {
                    errcode = -1,
                    errmsg = ex.Message,
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
            return result;
        }
        [HttpPost]
        public ActionResult Load()
        {
            HttpRequest request = HttpContext.Current.Request;
            ActionResult result;
            try
            {
                string value = request.Form["Params"];
                dynamic param = JsonConvert.DeserializeObject(value);
                string appDomainID = param.ID;
                AddInService addInService = new AddInService();
                addInService.InitPath();
                List<Common.AddIn> addIn = addInService.GetAddIn();
                List<Common.AddIn> list = new List<Common.AddIn>();
                foreach (Common.AddIn current in addIn)
                {
                    if (current.ID == Convert.ToInt32(appDomainID))
                    {
                        list.Add(current);
                    }
                }
                addInService.LoadAddIn(list);
                SqlHelper sqlHelper = new SqlHelper();
                sqlHelper.ExecuteNonQuery("update T_AddIn set F_Status = 1 where F_ID=" + appDomainID);
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
                }, JsonRequestBehavior.DenyGet);
            }
            catch (Exception ex)
            {
                Log.log(ex.ToString());
                result = this.Json(new
                {
                    errcode = -1,
                    errmsg = ex.Message,
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
            return result;
        }
        [HttpPost]
        public ActionResult FileUpload()
        {
            HttpRequest request = HttpContext.Current.Request;
            ActionResult result;
            try
            {
                if (request.Files.Count > 0)
                {
                    string value = request.Form["Params"];
                    dynamic param = JsonConvert.DeserializeObject(value);
                    string appDomainID = param.ID;
                    SqlHelper sqlHelper = new SqlHelper();
                    DataTable dataTable = sqlHelper.ExecuteDataTable("select * from T_AddIn where F_ID =" + appDomainID, new SqlParameter[0]);
                    if (dataTable != null && dataTable.Rows.Count > 0)
                    {
                        foreach (DataRow dataRow in dataTable.Rows)
                        {
                            string text2 = ConfigurationManager.AppSettings["AddInPath"].ToString() + "\\" + dataRow["F_AddIn_Path"].ToString() + "\\";
                            if (!Directory.Exists(text2))
                            {
                                Directory.CreateDirectory(text2);
                            }
                            foreach (HttpPostedFile httpPostedFile in request.Files)
                            {
                                text2 += httpPostedFile.FileName;
                                httpPostedFile.SaveAs(text2);
                            }
                        }
                        result = this.Json(new
                        {
                            errcode = 200,
                            errmsg = "OK",
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
                    else
                    {
                        result = this.Json(new
                        {
                            errcode = 404,
                            errmsg = "NOT FOUND",
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
                }
                else
                {
                    result = this.Json(new
                    {
                        errcode = 400,
                        errmsg = "Bad Request",
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
            }
            catch (Exception ex)
            {
                Log.log(ex.ToString());
                result = this.Json(new
                {
                    errcode = -1,
                    errmsg = ex.Message,
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
            return result;
        }
    }
}
