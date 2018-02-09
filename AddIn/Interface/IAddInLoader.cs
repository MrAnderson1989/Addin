using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AddIn.Interface
{
    public interface IAddInLoader
    {
        bool LoadAssembly(string Path);

        object Execute(string Controller, string Action, object[] Parameters);

        List<IController> GetControllers();
    }
}
