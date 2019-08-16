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
            ExcuteSubQueries(context, queryConfig, valueSetter, datas);
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

        public void ExcuteSubQueries(Context context, QueryConfig queryConfig, IValueSetter valueSetter, IEnumerable<object> parents)
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

            foreach (var relatedQueryConfig in relatedQueryConfigs)
            {
                IQueryHandler queryHandler = QueryHandlerFactory.GetQueryHandler(relatedQueryConfig.QueryType);
                var subContext = new Context(context)
                {
                    Configs = new QueryConfig[] { relatedQueryConfig }
                };

                context.Childs.Add(subContext);
                var obj = queryHandler.Query(subContext);

                if (relatedQueryConfig.CompareFields == null || relatedQueryConfig.CompareFields.Any() == false)
                {
                    var field = relatedQueryConfig.Filed;
                    foreach (var parent in parents)
                    {
                        valueSetter.SetField(relatedQueryConfig, parent, obj, field);
                    }

                    continue;
                }

                var mf = relatedQueryConfig.CompareFields.Select(cf => cf.Split(SqlKeyWorld.Split)).ToDictionary(c => c[0], c => c[1]);
                var datas = (IEnumerable<object>)obj;
                foreach (var parent in parents)
                {
                    var matchDatas = ValueGetter.GetFilterValues(mf, parent, datas);
                    valueSetter.SetField(relatedQueryConfig, parent, matchDatas, relatedQueryConfig.Filed);
                }
            }
        }
    }
}
