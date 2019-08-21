using System.Collections.Generic;
using System.Linq;
using Tim.SqlEngine.Models;

namespace Tim.SqlEngine.SqlHelper.QueryHandler
{
    public class SingleFieldQueryHandler : BaseQueryHandler
    {
        public override int Type => 3;

        public override object Query(Context context)
        {
            var queryConfig = context.Config;
            var handlerConfig = context.HandlerConfig;
            if (string.IsNullOrEmpty(queryConfig.Connection))
            {
                queryConfig.Connection = handlerConfig.Connection;
            }

            return SqlExcuter.ExcuteScalar(context);
        }
    }
}
