﻿using Newtonsoft.Json;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using Tim.SqlEngine.Convert;
using Tim.SqlEngine.Models;
using Tim.SqlEngine.Parser;
using Tim.SqlEngine.SqlHelper;
using Tim.SqlEngine.SqlHelper.UpdateHandler;

namespace Tim.SqlEngine
{
    public static partial class SqlEnginer
    {
        public static object Update(UpdateHandlerConfig handlerConfig, IDictionary<string, object> queryParams = null)
        {
            return Update(handlerConfig, null, queryParams);
        }

        public static object Update(UpdateHandlerConfig handlerConfig, object complexData, IDictionary<string, object> queryParams = null)
        {
            IUpdateHandler updateHandler = UpdateHandlerFactory.GetUpdateHandler(handlerConfig.QueryType);
            var context = new UpdateContext
            {
                HandlerConfig = handlerConfig,
                Configs = handlerConfig.Configs,
                Params = queryParams,
                ComplexData = complexData
            };

            ParamConvertUtil.DoConvert(context);
            var result = updateHandler.Update(context);
            if (context.Conns != null && context.Conns.Any())
            {
                context.Submit();
            }

            return result;
        }

        public static object Update(string name, IDictionary<string, object> queryParams = null)
        {
            return Update(name, null, queryParams);
        }

        public static object Update(string name, object complexData, IDictionary<string, object> queryParams = null)
        {
            UpdateHandlerConfig handlerConfig = JsonParser.ReadHandlerConfig<UpdateHandlerConfig>(name);
            return Update(handlerConfig, complexData, queryParams);
        }

        public static object Update(string name, string jsonData, IDictionary<string, object> queryParams = null)
        {
            UpdateHandlerConfig handlerConfig = JsonParser.ReadHandlerConfig<UpdateHandlerConfig>(name);
            if (string.IsNullOrEmpty(handlerConfig.JType) && 
                string.IsNullOrEmpty(jsonData))
            {
                return Update(handlerConfig, null, queryParams);
            }

            if (!string.IsNullOrEmpty(handlerConfig.JType))
            {
                var typeStrs = handlerConfig.JType.Split(SqlKeyWorld.Split3);
                var instanceType = ReflectUtil.ReflectUtil.CreateType(typeStrs[0], typeStrs[1]);
                var instance = JsonParser.CreateInstance(jsonData, instanceType);
                return Update(handlerConfig, instance, queryParams);
            }

            if (!jsonData.StartsWith("["))
            {
                return Update(handlerConfig, JsonConvert.DeserializeObject<ExpandoObject>(jsonData), queryParams);
            }

            var datas = JsonConvert.DeserializeObject<IEnumerable<ExpandoObject>>(jsonData);
            List<object> objects = new List<object>(datas);
            return Update(handlerConfig, datas, queryParams);
        }
    }
}
