using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using Tim.SqlEngine.Models;
using Tim.SqlEngine.SqlHelper.QueryHandler.ReleatedFillHandler;
using Tim.SqlEngine.ValueSetter;

namespace Tim.SqlEngine.SqlHelper.QueryHandler
{
    public class SimpleQueryHandler : BaseQueryHandler
    {
        public override int Type => 1;

        protected override object DoQuery(Context context)
        {
            var queryConfig = context.Config;
            var handlerConfig = context.HandlerConfig;
            if (string.IsNullOrEmpty(queryConfig.Connection))
            {
                queryConfig.Connection = handlerConfig.Connection;
            }

            IValueSetter valueSetter = queryConfig.Create();
            var datas = SqlExcuter.ExcuteQuery(context, valueSetter);
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
                queryConfig.Config["related_queries"] == null ||
                parents.Any() == false)
            {
                return;
            }

            var relatedQueryConfigs = JsonConvert.DeserializeObject<IEnumerable<ReleatedQuery>>(queryConfig.Config["related_queries"].ToString());
            if (relatedQueryConfigs.Any() == false)
            {
                return;
            }

            IDictionary<string, object> contentData = new ExpandoObject();
            foreach (var relatedQueryConfig in relatedQueryConfigs)
            {
                IQueryHandler queryHandler = QueryHandlerFactory.GetQueryHandler(relatedQueryConfig.QueryType);
                IConditionQueryHandler conditionQueryHandler = queryHandler as IConditionQueryHandler;

                var subContext = new Context(context)
                {
                    Data = contentData,
                    Configs = new QueryConfig[] { relatedQueryConfig }
                };

                if (conditionQueryHandler != null)
                {
                    var isContinue = conditionQueryHandler.Continue(subContext);
                    if (isContinue == false)
                    {
                        if (conditionQueryHandler.WhetheStop(subContext))
                        {
                            return;
                        }

                        continue;
                    }
                }

                context.Childs.Add(subContext);
                var obj = queryHandler.Query(subContext);
                if (!string.IsNullOrEmpty(relatedQueryConfig.Filed))
                {
                    contentData.Add(relatedQueryConfig.Filed, obj);
                }

                if (conditionQueryHandler != null)
                {
                    var result = conditionQueryHandler.WhetheResultStop(subContext, obj);
                    if (result == true)
                    {
                        return;
                    }
                }

                if (relatedQueryConfig.IngoreFill)
                {
                    continue;
                }

                SetSubQueryValue(relatedQueryConfig, valueSetter, parents, (IEnumerable<object>)obj);
            }
        }

        private void SetSubQueryValue(ReleatedQuery config, IValueSetter valueSetter, IEnumerable<object> parents, IEnumerable<object> datas)
        {
            var compareFields = config.CompareFields ?? new string[] { };
            Dictionary<string, string> mf = compareFields.Select(cf => cf.Split(SqlKeyWorld.Split)).ToDictionary(c => c[0], c => c[1]);
            var matchOneTime = config.MatchOneTime;

            foreach (var parent in parents)
            {
                IEnumerable<object> matchDatas = ValueGetter.GetFilterValues(mf, parent, datas);
                var handler = ReleatedFillHandlerFactory.Create(config);
                var data = handler.Fill(config, parent, matchDatas, valueSetter);
                if (matchOneTime == false)
                {
                    continue;
                }

                datas = datas.Except(matchDatas).ToArray();
            }
        }
    }
}
