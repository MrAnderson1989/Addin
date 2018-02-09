using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AddIn.Interface
{
    public interface IParameter
    {
        string ParameterName { get; set; }
        Type ParameterType { get; set; }
    }
}
