using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AddIn.Web.Routing
{
    public interface IActionHttpMethodProvider
    {
        IEnumerable<string> HttpMethods { get; }
    }
}
