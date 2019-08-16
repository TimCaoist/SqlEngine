using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            var queryParam = context.ExcutedQueryParams;
            IValueSetter valueSetter = handlerConfig.Create();
            object outData = valueSetter.CreateInstance();
            context.Data = outData;

            foreach (var queryConfig in queryConfigs)
            {
                IQueryHandler queryHandler = QueryHandlerFactory.GetQueryHandler(queryConfig.QueryType);
                var subContext = new Context(context)
                {
                    Configs = new QueryConfig[] { queryConfig }
                };

                context.Childs.Add(subContext);
                var data = queryHandler.Query(subContext);
                valueSetter.SetterField(queryConfig.Filed, data);
            }

            return outData;
        }
    }
}