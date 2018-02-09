using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AddIn.Interface
{
    public interface IAction
    {
        string ActionName { get; set; }

        string HttpMethod { get; set; }

        string Description { get; set; }

        List<IParameter> Parameters { get; set; }

        Type ReturnType { get; set; }
    }
}
