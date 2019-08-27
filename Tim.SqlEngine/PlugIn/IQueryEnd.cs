using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tim.SqlEngine.Models;

namespace Tim.SqlEngine.PlugIn
{
    public interface IQueryEnd
    {
        object Handler(BaseHadlerConfig config, object data, IDictionary<string, object> queryParams);
    }
}
