using AddIn.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AddIn.Entity
{
    public class AddInParameter : MarshalByRefObject,IParameter
    {
        public string ParameterName { get; set ; }
        public Type ParameterType { get ; set ; }
    }
}
