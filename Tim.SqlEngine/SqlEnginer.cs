﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tim.SqlEngine.Parser;
using Tim.SqlEngine.Models;
using Tim.SqlEngine.SqlHelper.QueryHandler;

namespace Tim.SqlEngine
{
    public static class SqlEnginer
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
                QueryParams = queryParams
            };

            return queryHandler.Query(context);
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
    }
}
