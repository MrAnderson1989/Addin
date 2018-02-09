using AddIn.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace AddIn.Entity
{
    public static class AddInManager
    {
        static AddInManager()
        {
            AppDomains = new Dictionary<string, AppDomain>();
            Loaders = new Dictionary<string, IAddInLoader>();
        }

        public static Dictionary<string, AppDomain> AppDomains { get; set; }
        public static Dictionary<string, IAddInLoader> Loaders { get; set; }

        public static AppDomain CreateAddInAppDomain(string AddInID)
        {
            try
            {

                string path = System.AppDomain.CurrentDomain.BaseDirectory + "\\bin\\AddIn.dll";

                //Create evidence for the new application domain from evidence of
                Evidence adEvidence = AppDomain.CurrentDomain.Evidence;

                AppDomainSetup ads = GetAppDomainSetup();

                AppDomain NewDomain = AppDomain.CreateDomain(AddInID, adEvidence, ads);
                //string friendlyName = myDomain.FriendlyName;
                if (!AppDomains.ContainsKey(AddInID))
                {
                    AppDomains.Add(AddInID, NewDomain);
                }
                else
                {
                    AppDomains[AddInID] = NewDomain;
                }
                //IObject obj = (IObject)myDomain.CreateInstanceFromAndUnwrap(path, "Interface.AddInLoader");
                IAddInLoader obj = (IAddInLoader)NewDomain.CreateInstanceFromAndUnwrap(path, "AddIn.Entity.AddInLoader");
                if (!Loaders.ContainsKey(AddInID))
                {
                    Loaders.Add(AddInID, obj);
                }
                else
                {
                    Loaders[AddInID] = obj;
                }
                return NewDomain;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private static AppDomainSetup  GetAppDomainSetup()
        {
            AppDomainSetup ads = new AppDomainSetup();
            ads.ApplicationName = "AddInService";
            //应用程序根目录
            ads.ApplicationBase = AppDomain.CurrentDomain.BaseDirectory;
            //子目录（相对形式）在AppDomainSetup中加入外部程序集的所在目录，多个目录用分号间隔
            ads.PrivateBinPath = AppDomain.CurrentDomain.BaseDirectory + "\\ExtendAssembly\\";
            //设置缓存目录
            ads.CachePath = ads.ApplicationBase;
            //获取或设置指示影像复制是打开还是关闭
            ads.ShadowCopyFiles = "true";
            //获取或设置目录的名称，这些目录包含要影像复制的程序集
            ads.ShadowCopyDirectories = ads.ApplicationBase;

            ads.DisallowBindingRedirects = false;

            ads.DisallowCodeDownload = true;

            ads.ConfigurationFile = AppDomain.CurrentDomain.SetupInformation.ConfigurationFile;

            return ads;
        }

        public static object Execute(string AddInID, string Controller, string Action, object[] Parameters)
        {
            if (Loaders.ContainsKey(AddInID))
            {
                try
                {
                    if (Loaders.ContainsKey(AddInID) && Loaders[AddInID] != null)
                    {
                        return Loaders[AddInID].Execute(Controller, Action, Parameters);
                    }
                    else
                    {
                        throw new Exception("AddInLoader已被回收！");
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            else
            {
                throw new Exception("找不到插件域！");
            }
        }

        public static bool LoadAddInDll(string AddInID, string DllPath)
        {
            try
            {
                if (Loaders.ContainsKey(AddInID))
                {
                    if (Loaders.ContainsKey(AddInID) && Loaders[AddInID] != null)
                    {
                        return Loaders[AddInID].LoadAssembly(DllPath);
                    }
                    else
                    {
                        throw new Exception("AddInLoader已被回收！");
                    }
                }
                else
                {
                    throw new Exception("找不到插件域！");
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static bool UnloadAddIn(string AddInID)
        {
            try
            {
                if (AppDomains.ContainsKey(AddInID))
                {
                    AppDomain.Unload(AppDomains[AddInID]);
                    return true;
                }
                else
                {
                    throw new Exception("应用程序域已被卸载！");
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static List<IController> GetAddInControllers(string AddInID)
        {
            try
            {
                if (Loaders.ContainsKey(AddInID))
                {
                    if (Loaders.ContainsKey(AddInID) && Loaders[AddInID] != null)
                    {
                        return Loaders[AddInID].GetControllers();
                    }
                    else
                    {
                        throw new Exception("AddInLoader已被回收！");
                    }
                }
                else
                {
                    throw new Exception("找不到插件域！");
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    }
}
