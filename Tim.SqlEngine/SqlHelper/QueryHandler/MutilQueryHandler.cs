using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tim.SqlEngine.Models;

namespace Tim.SqlEngine.SqlHelper.QueryHandler
{
    public class MutilQueryHandler : BaseQueryHandler
    {
        public override int Type => 2;

        public override object Query(HandlerConfig handlerConfig, IDictionary<string, object> queryParam)
        {
            var queryConfigs = handlerConfig.Configs;
            IDictionary<string, object> outData = new ExpandoObject();
            foreach (var queryConfig in queryConfigs)
            {
                IQueryHandler queryHandler = QueryHandlerFactory.GetQueryHandler(queryConfig.QueryType);
                var data = queryHandler.Query(handlerConfig, queryConfig, queryParam);
                outData.Add(queryConfig.Filed, data);
            }

            if (handlerConfig.Config == null)
            {
                return outData;
            }

            object instance = ReflectUtil.ReflectUtil.CreateInstance(handlerConfig.Config["assembly_str"].ToString(), handlerConfig.Config["type_str"].ToString());
            var type = instance.GetType();
            foreach (var data in outData)
            {
                var p = type.GetProperty(data.Key);
                if (p == null)
                {
                    continue;
                }

                p.SetValue(instance, data.Value);
            }

            return instance;
        }

        public override object Query(HandlerConfig handlerConfig, QueryConfig config, IDictionary<string, object> queryParam)
        {
            throw new NotImplementedException();
        }
    }
}
