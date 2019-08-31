using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tim.SqlEngine.Common;
using Tim.SqlEngine.Models;
using Tim.SqlEngine.ValueSetter;

namespace Tim.SqlEngine.Parser.ParamHandler
{
    public class ObjectParamHandler : IParamHandler
    {
        protected virtual object GetObject(string objectKey, IContext context)
        {
            var queryParams = context.Params;
            object data;
            if (!queryParams.TryGetValue(objectKey, out data))
            {
                throw new ArgumentNullException(string.Concat("参数", objectKey, "不存在!"));
            }

            return data;
        }

        protected virtual IDictionary<string, object> GetQueryParams(IContext context)
        {
            return context.Params;
        }


        public ParamInfo GetParamInfo(IContext context, string dataStr)
        {
            var key = dataStr.Substring(1, dataStr.Length - 1);
            var keyArray = key.Split('_');
            var str = keyArray[1];
            var array = str.Split('.');
            if (array.Count() <= 1)
            {
                throw new ArgumentException(string.Concat("参数", str, "设置错误!"));
            }

            var objectKey = array[0];
            var fields = array.Skip(1).ToArray();
            var realKey = string.Concat(objectKey, "_",  string.Join("_", fields));

            var queryParams = GetQueryParams(context);

            object outData = GetDataFromCache(queryParams, realKey);
            var data = GetObject(objectKey, context);
            var valueInfo = ValueGetter.GetValue(fields, data);
            outData = ValueGetter.Builder(valueInfo);
            queryParams.ReplaceOrInsert(realKey, outData);

            return new ParamInfo
            {
                OriginalData = valueInfo.Data,
                Type = ParamType.Object,
                Name = key,
                Data = outData
            };
        }

        protected virtual object GetDataFromCache(IDictionary<string, object> queryParams, string realKey) {
            object outData;
            if (queryParams.TryGetValue(realKey, out outData))
            {
                return outData;
            }

            return null;
        }

        public virtual bool Match(string paramStr)
        {
            return paramStr.StartsWith(SqlKeyWorld.ObjectStart, StringComparison.OrdinalIgnoreCase);
        }
    }
}
