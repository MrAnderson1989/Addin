using AddIn.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AddIn.Entity
{
    public class AddInAction : MarshalByRefObject,IAction
    {
        public AddInAction()
        {
            Parameters = new List<IParameter>();
        }
        public string ActionName { get ; set  ; }
        public List<IParameter> Parameters { get; set; }
        public Type ReturnType { get; set ; }

        public string Description { get; set; } = "没有API描述信息。";

        public string HttpMethod { get; set; } = "GET";
    }
}
