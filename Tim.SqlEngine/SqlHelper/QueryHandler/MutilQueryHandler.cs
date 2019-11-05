using System.Collections.Generic;
using System.Dynamic;
using Tim.CacheUtil.Models;
using Tim.SqlEngine.Models;
using Tim.SqlEngine.ValueSetter;

namespace Tim.SqlEngine.SqlHelper.QueryHandler
{
    /// <summary>
    /// 组合查询
    /// </summary>
    public class MutilQueryHandler : BaseQueryHandler
    {
        public override int Type => 2;

        protected override CacheConfig GetCacheConfig(Context context)
        {
            return context.HandlerConfig.CacheConfig;
        }

        protected override object DoQuery(Context context)
        {
            var handlerConfig = context.HandlerConfig;
            var queryConfigs = context.Configs;
            var queryParam = context.Params;
            IValueSetter valueSetter = handlerConfig.Create();

            object outData = valueSetter.CreateInstance();
            context.Data = outData;

            IDictionary<string, object> contentData = new ExpandoObject();
            IValueSetter contentSetter = ValueSetterCreater.Create(contentData);

            foreach (var queryConfig in queryConfigs)
            {
                IQueryHandler queryHandler = QueryHandlerFactory.GetQueryHandler(queryConfig.QueryType);
                IConditionQueryHandler conditionQueryHandler = queryHandler as IConditionQueryHandler;
                var subContext = new Context(context)
                {
                    Data = contentData,
                    Configs = new QueryConfig[] { queryConfig }
                };

                if (conditionQueryHandler != null)
                {
                    var isContinue = conditionQueryHandler.Continue(subContext);
                    if (isContinue == false)
                    {
                        if (conditionQueryHandler.WhetheStop(subContext))
                        {
                            return outData;
                        }

                        continue;
                    }
                }

                context.Childs.Add(subContext);
                var data = queryHandler.Query(subContext);
                contentSetter.SetFieldByConfig(contentData, data, queryConfig);
                if (conditionQueryHandler != null)
                {
                    var result = conditionQueryHandler.WhetheResultStop(subContext, data);
                    if (result == true)
                    {
                        return outData;
                    }
                }

                if (queryConfig.IngoreFill == true)
                {
                    continue;
                }

                valueSetter.SetField(queryConfig.Filed, data);
            }

            return outData;
        }
    }
}