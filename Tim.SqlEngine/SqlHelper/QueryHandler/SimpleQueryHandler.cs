using System.Collections.Generic;
using System.Linq;
using Tim.SqlEngine.Models;
using Tim.SqlEngine.ValueSetter;

namespace Tim.SqlEngine.SqlHelper.QueryHandler
{
    public class SimpleQueryHandler : BaseQueryHandler
    {
        public override int Type => 1;

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

            IValueSetter valueSetter;
            if (queryConfig.Config == null)
            {
                valueSetter = new DynamicValueSetter();
            }
            else
            {
                valueSetter = new ReflectValueSetter(queryConfig.Config["assembly_str"].ToString(), queryConfig.Config["type_str"].ToString());
            }

            var datas = SqlQueryExcuter.ExcuteQuery(queryConfig, valueSetter, queryParams);
            if (queryConfig.OnlyOne)
            {
                return datas.First();
            }

            return datas;
        }
    }
}
