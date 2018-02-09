using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AddIn.Interface
{
    public interface IAddInManager
    {
        Dictionary<string, AppDomain> AppDomains { get; set; }

        Dictionary<string, IAddInLoader> Loaders { get; set; }

        AppDomain CreateAddInAppDomain(string AddInID);

        bool LoadAddInDll(string AddInID, string DllPath);

        bool UnloadAddIn(string AddInID);

        object Execute(string AddInID,string Controller,string Action,object[] Parameters);

    }
}
