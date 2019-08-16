using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Linq;
using Tim.SqlEngine.Models;
using Tim.SqlEngine.ValueSetter;

namespace Tim.SqlEngine.SqlHelper.QueryHandler
{
    public class SimpleQueryHandler : BaseQueryHandler
    {
        public override int Type => 1;

        public override object Query(Context context)
        {
            var queryConfig = context.Config;
            var handlerConfig = context.HandlerConfig;
            if (string.IsNullOrEmpty(queryConfig.Connection))
            {
                queryConfig.Connection = handlerConfig.Connection;
            }

            IValueSetter valueSetter = queryConfig.Create();
            var datas = SqlQueryExcuter.ExcuteQuery(context, valueSetter);
            context.Data = datas;
            ExcuteSubQueries(context, queryConfig);
            if (!queryConfig.OnlyOne)
            {
                return datas;
            }

            if (datas.Any())
            {
                return datas.First();
            }

            return new object();
        }

        public void ExcuteSubQueries(Context context, QueryConfig queryConfig)
        {
            if (queryConfig.Config == null || 
                queryConfig.Config["related_queries"] == null)
            {
                return;
            }

            var relatedQueryConfigs = JsonConvert.DeserializeObject<IEnumerable<ReleatedQuery>>(queryConfig.Config["related_queries"].ToString());
            if (relatedQueryConfigs.Any() == false)
            {
                return;
            }

            var queryParams = context.ExcutedQueryParams;
            var newParams = new Dictionary<string, object>();
            foreach (var item in queryParams)
            {
                newParams.Add(item.Key, item.Value);
            }

            newParams.Add(SqlKeyWorld.ParnetKey, context.Data);

            foreach (var relatedQueryConfig in relatedQueryConfigs)
            {
                IQueryHandler queryHandler = QueryHandlerFactory.GetQueryHandler(relatedQueryConfig.QueryType);
                var subContext = new Context(context)
                {
                    Configs = new QueryConfig[] { relatedQueryConfig },
                    ExcutedQueryParams = newParams
                };

                context.Childs.Add(subContext);
            }
        }
    }
}
