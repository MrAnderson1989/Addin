using AddIn.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AddIn.Entity
{
    public class AddInController : MarshalByRefObject,IController
    {
        public AddInController()
        {
            Actions = new List<IAction>();
        }
        public string ControllerName { get; set; }
        public List<IAction> Actions { get ; set ; }
    }
}
