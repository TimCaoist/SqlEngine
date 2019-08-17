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
                SetSubQueryValue(relatedQueryConfig, valueSetter, parents, (IEnumerable<object>)obj);
            }
        }

        private void SetSubQueryValue(ReleatedQuery config, IValueSetter valueSetter, IEnumerable<object> parents, IEnumerable<object> datas)
        {
            var compareFields = config.CompareFields ?? new string[] { };
            Dictionary<string, string> mf = compareFields.Select(cf => cf.Split(SqlKeyWorld.Split)).ToDictionary(c => c[0], c => c[1]);
            var isSingle = config.IsSingle;
            var defaultValue = config.UsedFieldDefaultValue;
            foreach (var parent in parents)
            {
                IEnumerable<object> matchDatas = ValueGetter.GetFilterValues(mf, parent, datas);
                object matchData = matchDatas.FirstOrDefault();

                if (string.IsNullOrEmpty(config.UsedField))
                {
                    if (isSingle)
                    {
                        valueSetter.SetField(parent, matchData, config.Filed);
                        continue;
                    }

                    valueSetter.SetField(parent, matchDatas, config.Filed);
                    continue;
                }

                if (!isSingle)
                {
                    matchData = ValueGetter.GetValue(config.UsedField, matchDatas).Data;
                }
                else if (matchData != null)
                {
                    matchData = ValueGetter.GetValue(config.UsedField, matchData).Data ?? defaultValue;
                }
                else
                {
                    matchData = defaultValue;
                }

                valueSetter.SetField(parent, matchData, config.Filed);
                continue;
            }
        }
    }
}
