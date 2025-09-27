using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FTM.Domain.Specification
{
    public class PropertyFilter
    {
        public string Name { get; set; }
        public string Operation { get; set; }
        public object Value { get; set; }
    }
}
