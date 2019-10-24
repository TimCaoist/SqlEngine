using System;
using System.Collections.Generic;
using System.Linq;
using Tim.SqlEngine.Common;
using Tim.SqlEngine.Convert;
using Tim.SqlEngine.Models;
using Tim.SqlEngine.Parser;
using Tim.SqlEngine.PlugIn;
using Tim.SqlEngine.SqlHelper.QueryHandler;

namespace Tim.SqlEngine
{
    public static partial class SqlEnginer
    {
        
        public static object Query(HandlerConfig handlerConfig, IDictionary<string, object> queryParams = null)
        {
            return Query(handlerConfig, null, queryParams);
        }

        public static object Query(HandlerConfig handlerConfig, object complexData, IDictionary<string, object> queryParams = null)
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
                Params = queryParams,
                ComplexData = complexData,
            };

            ParamConvertUtil.DoConvert(context);
            var returnData = queryHandler.Query(context);
            //执行完查询后回调
            return handlerConfig.OnQueryEnd(returnData, queryParams);
        }

        public static object Query(string name, object complexData, IDictionary<string, object> queryParams = null)
        {
            HandlerConfig handlerConfig = JsonParser.ReadHandlerConfig<HandlerConfig>(name);
            return Query(handlerConfig, complexData, queryParams);
        }

        public static object Query(string name, IDictionary<string, object> queryParams = null)
        {
            return Query(name, null, queryParams);
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

            HandlerConfig handlerConfig = JsonParser.ReadHandlerConfig<HandlerConfig>(name);
            return Query(handlerConfig, null, queryParams);
        }

        public static object Query(string name, string data, string assemblyString, string typeStr)
        {
            var type = ReflectUtil.ReflectUtil.CreateType(assemblyString, typeStr);
            return Query(name, JsonParser.CreateInstance(data, type));
        }

        public static IEnumerable<TData> ToDatas<TData>(this object obj)
        {
            return DataConvert.ToDatas<TData>(obj);
        }

    }
}
