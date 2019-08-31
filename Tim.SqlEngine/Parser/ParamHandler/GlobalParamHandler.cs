using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tim.SqlEngine.Models;
using Tim.SqlEngine.PlugIn;

namespace Tim.SqlEngine.Parser.ParamHandler
{
    internal class GlobalParamHandler : IParamHandler
    {
        public ParamInfo GetParamInfo(IContext context, string dataStr)
        {
            var key = dataStr.Substring(1, dataStr.Length - 1);
            var keyArray = dataStr.Split('_');
            var realKey = keyArray[1];
            var data = SqlEnginerConfig.GetGlobalDatas(realKey);
            if (data == null)
            {
                throw new ArgumentNullException(string.Concat("不存在", realKey, "全局对象!"));
            }

            var queryParams = context.Params;
            object outData;
            if (!queryParams.TryGetValue(key, out outData))
            {
                var convertData = ParamsUtil.CovnertParam(data, queryParams, realKey);
                if (convertData != data)
                {
                    queryParams.Add(key, convertData);
                    data = convertData;
                }
            }
            else {
                data = outData;
            }

            return new ParamInfo
            {
                Type = ParamType.Global,
                Name = key,
                Data = data
            };
        }

        public bool Match(string paramStr)
        {
            return paramStr.StartsWith(SqlKeyWorld.GlobalStart, StringComparison.OrdinalIgnoreCase);
        }
    }
}
