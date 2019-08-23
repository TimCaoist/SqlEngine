using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tim.SqlEngine.Models
{
    public class ValueInfo
    {
        public bool IsArray { get; internal set; }
        public object Data { get; internal set; }
    }
}
