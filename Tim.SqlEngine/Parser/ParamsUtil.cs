using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Tim.SqlEngine.Models;
using Tim.SqlEngine.Parser.ParamHandler;
using Tim.SqlEngine.PlugIn;

namespace Tim.SqlEngine.Parser
{
    public static class ParamsUtil
    {
        public static IEnumerable<ParamInfo> GetParams(IContext context, System.Text.RegularExpressions.MatchCollection matches)
        {
            ICollection<ParamInfo> paramInfos = new List<ParamInfo>();
            foreach (Match match in matches)
            {
                var paramInfo = GetParamData(context, match.ToString().TrimEnd(')', '(' , ' ', SqlKeyWorld.Split, SqlKeyWorld.Split3));
                if (paramInfo == null)
                {
                    continue;
                }

                paramInfo.Match = match;
                paramInfos.Add(paramInfo);
            }

            return paramInfos;
        }

        public static ParamInfo GetParamData(IContext context, string dataStr)
        {
            var paramHandler = ParamHandlerFactory.Find(dataStr);
            return paramHandler.GetParamInfo(context, dataStr);
        }

        internal static IDictionary<string, object> Convert(IEnumerable<ParamInfo> usedParams)
        {
            IDictionary<string, object> datas = new Dictionary<string, object>();
            foreach (var p in usedParams)
            {
                if (datas.ContainsKey(p.Name))
                {
                    continue;
                }

                datas.Add(p.Name, p.Data);
            }

            return datas;
        }

        public static ParamInfo GetGlobalParamData(IDictionary<string, object> queryParams, string key)
        {
            var keyArray = key.Split('_');
            var realKey = keyArray[1]; 
            var data = SqlEnginerConfig.GetGlobalDatas(realKey);
            if (data == null)
            {
                throw new ArgumentNullException(string.Concat("不存在", realKey , "全局对象!"));
            }

            object outData = null;
            if (!queryParams.TryGetValue(key, out outData))
            {
                var convertData = CovnertParam(data, queryParams, realKey);
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

        public static object CovnertParam(object data, IDictionary<string, object> queryParams, string key)
        {
            var gobalValue = data as IGobalValue;
            if (gobalValue != null)
            {
                return gobalValue.GetValue(queryParams, key);
            }

            var func = data as Func<IDictionary<string, object>, string, object>;
            if (func != null)
            {
                return func.Invoke(queryParams, key);
            }

            return data;
        }
    }
}
