using System.Collections.Generic;
using System.Dynamic;
using Tim.SqlEngine.Models;
using Tim.SqlEngine.ValueSetter;

namespace Tim.SqlEngine.SqlHelper.QueryHandler
{
    public class MutilQueryHandler : BaseQueryHandler
    {
        public override int Type => 2;

        public override object Query(Context context)
        {
            var handlerConfig = context.HandlerConfig;
            var queryConfigs = context.Configs;
            var queryParam = context.Params;
            IValueSetter valueSetter = handlerConfig.Create();

            object outData = valueSetter.CreateInstance();
            context.Data = outData;

            IDictionary<string, object> contentData = new ExpandoObject();

            foreach (var queryConfig in queryConfigs)
            {
                IQueryHandler queryHandler = QueryHandlerFactory.GetQueryHandler(queryConfig.QueryType);
                var subContext = new Context(context)
                {
                    Data = contentData,
                    Configs = new QueryConfig[] { queryConfig }
                };

                context.Childs.Add(subContext);
                var data = queryHandler.Query(subContext);
                contentData.Add(queryConfig.Filed, data);

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