using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tim.SqlEngine.Parser;
using Tim.SqlEngine.Models;
using Tim.SqlEngine.SqlHelper.QueryHandler;
using Tim.SqlEngine.PlugIn;

namespace Tim.SqlEngine
{
    public static partial class SqlEnginer
    {
        
        public static object Query(HandlerConfig handlerConfig, IDictionary<string, object> queryParams = null)
        {
            IQueryHandler queryHandler = QueryHandlerFactory.GetQueryHandler(handlerConfig.QueryType);
            if (handlerConfig.Configs == null || handlerConfig.Configs.Any() == false)
            {
                throw new ArgumentNullException("handlerConfig.Configs");
            }

            var context = new Context
            {
                HandlerConfig = handlerConfig,
                Configs = handlerConfig.Configs,
                Params = queryParams
            };

            var returnData = queryHandler.Query(context);
            return handlerConfig.OnQueryEnd(returnData, queryParams);
        }

        public static object Query(string name, IDictionary<string, object> queryParams = null)
        {
            HandlerConfig handlerConfig = JsonParser.ReadHandlerConfig(name);
            return Query(handlerConfig, queryParams);
        }

        public static object Query(string name, object param)
        {
            var type = param.GetType();
            IDictionary<string, object> queryParams = new Dictionary<string, object>();
            foreach (var p in type.GetProperties())
            {
                var val = p.GetValue(param);
                queryParams.Add(p.Name, val);
            }

            HandlerConfig handlerConfig = JsonParser.ReadHandlerConfig(name);
            return Query(handlerConfig, queryParams);
        }

        public static object Query(string name, string data, string assemblyString, string typeStr)
        {
            var type = ReflectUtil.ReflectUtil.CreateType(assemblyString, typeStr);
            return Query(name, JsonParser.CreateInstance(data, type));
        }

        public static IEnumerable<TData> ToDatas<TData>(this object obj)
        {
            var datas = (IEnumerable<object>)obj;
            return datas.Cast<TData>();
        }

        public static TData ToSingleData<TData>(this object obj)
        {
            return (TData)obj;
        }
    }
}
