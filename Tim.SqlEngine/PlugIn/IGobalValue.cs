using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tim.SqlEngine.PlugIn
{
    public interface IGobalValue
    {
        object GetValue(IDictionary<string, object> queryParams, string key);
    }
}
