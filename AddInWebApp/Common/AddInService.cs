using AddIn.Entity;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
namespace AddInWebApp.Common
{
    public class AddInService
    {
        public void InitPath()
        {
            string text = ConfigurationManager.AppSettings["AddInPath"].ToString();
            if (!string.IsNullOrEmpty(text))
            {
                if (!Directory.Exists(text))
                {
                    Directory.CreateDirectory(text);
                    return;
                }
            }
            else
            {
                Log.log("插件目录为空");
            }
        }
        public List<AddIn> GetAddIn()
        {
            List<AddIn> result;
            try
            {
                string commandText = "select * from T_AddIn";
                SqlHelper sqlHelper = new SqlHelper();
                DataTable dataTable = sqlHelper.ExecuteDataTable(commandText, new SqlParameter[0]);
                List<AddIn> list = new List<AddIn>();
                if (dataTable != null && dataTable.Rows.Count > 0)
                {
                    foreach (DataRow dataRow in dataTable.Rows)
                    {
                        if (dataRow != null)
                        {
                            AddIn item = new AddIn
                            {
                                ID = Convert.ToInt32(dataRow["F_ID"]),
                                ControllerName = dataRow["F_AppDomain_Name"].ToString(),
                                DllName = dataRow["F_AddIn_Path"].ToString(),
                                AppDomainStatus = Convert.ToInt32(dataRow["F_Status"]),
                                Description = dataRow["F_Description"].ToString()
                            };
                            list.Add(item);
                        }
                    }
                }
                result = list;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return result;
        }
        public void LoadAddIn(List<AddIn> AddIns)
        {
            try
            {
                foreach (AddIn current in AddIns)
                {
                    if (current.AppDomainStatus == 1)
                    {
                        string text = ConfigurationManager.AppSettings["AddInPath"].ToString() + "\\" + current.ControllerName + "\\";
                        if (!Directory.Exists(text))
                        {
                            throw new Exception("插件路径错误：" + text);
                        }
                        AddInManager.CreateAddInAppDomain(current.ControllerName);
                        AddInManager.LoadAddInDll(current.ControllerName, text + current.DllName);
                    }
                }
            }
            catch (Exception ex)
            {
                Log.log(ex.ToString());
            }
        }


    }

    public class AddIn
    {
        public int ID
        {
            get;
            set;
        }
        public string ControllerName
        {
            get;
            set;
        }
        public string DllName
        {
            get;
            set;
        }
        public int AppDomainStatus
        {
            get;
            set;
        }
        public string Description
        {
            get;
            set;
        }
    }
}
