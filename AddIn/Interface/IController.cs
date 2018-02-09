using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AddIn.Interface
{
    public interface IController
    {
        string ControllerName { get; set; }
        List<IAction> Actions { get; set; }
    }
}
