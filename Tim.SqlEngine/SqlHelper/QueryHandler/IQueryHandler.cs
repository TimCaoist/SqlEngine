using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tim.SqlEngine.Models;

namespace Tim.SqlEngine.SqlHelper.QueryHandler
{
    public interface IQueryHandler
    {
        object Query(HandlerConfig handlerConfig, IDictionary<string, object> queryParams);

        object Query(HandlerConfig handlerConfig, QueryConfig config, IDictionary<string, object> queryParams);
    }
}
