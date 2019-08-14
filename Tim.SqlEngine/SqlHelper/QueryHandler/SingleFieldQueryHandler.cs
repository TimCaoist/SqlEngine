using System.Collections.Generic;
using System.Linq;
using Tim.SqlEngine.Models;

namespace Tim.SqlEngine.SqlHelper.QueryHandler
{
    public class SingleFieldQueryHandler : BaseQueryHandler
    {
        public override int Type => 3;

        public override object Query(HandlerConfig handlerConfig, IDictionary<string, object> queryParams)
        {
            var queryConfig = handlerConfig.Configs.First();
            return Query(handlerConfig, queryConfig, queryParams);
        }

        public override object Query(HandlerConfig handlerConfig, QueryConfig queryConfig, IDictionary<string, object> queryParams)
        {
            if (string.IsNullOrEmpty(queryConfig.Connection))
            {
                queryConfig.Connection = handlerConfig.Connection;
            }

            return SqlQueryExcuter.ExcuteScalar(queryConfig, queryParams);
        }
    }
}
