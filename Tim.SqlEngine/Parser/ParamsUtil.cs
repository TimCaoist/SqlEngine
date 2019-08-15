using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Tim.SqlEngine.Models;
using Tim.SqlEngine.PlugIn;

namespace Tim.SqlEngine.Parser
{
    public static class ParamsUtil
    {
        public static IEnumerable<ParamInfo> GetParams(System.Text.RegularExpressions.MatchCollection matches, IDictionary<string, object> queryParams)
        {
            ICollection<ParamInfo> paramInfos = new List<ParamInfo>();
            foreach (Match match in matches)
            {
                var paramInfo = GetParamData(queryParams, match.ToString().TrimEnd());
                paramInfo.Match = match;
                paramInfos.Add(paramInfo);
            }

            return paramInfos;
        }

        public static ParamInfo GetParamData(IDictionary<string, object> queryParams, string dataStr)
        {
            if (!dataStr.StartsWith(SqlKeyWorld.ParamStart))
            {
                return new ParamInfo
                {
                    Type = ParamType.Constant,
                    Name = string.Empty,
                    Data = dataStr
                };
            }

            var key = dataStr.Replace(SqlKeyWorld.ParamStart, string.Empty);
            if (key.StartsWith(SqlKeyWorld.GlobalStart, StringComparison.OrdinalIgnoreCase))
            {
                return GetGlobalParamData(queryParams, key);
            }

            object data;
            if (queryParams.TryGetValue(key, out data)) {
                return new ParamInfo
                {
                    Type = ParamType.Global,
                    Name = key,
                    Data = data
                };
            }

            throw new ArgumentNullException(string.Concat("参数", key, "不存在!"));
        }

        internal static IDictionary<string, object> Convert(IEnumerable<ParamInfo> usedParams)
        {
            IDictionary<string, object> datas = new Dictionary<string, object>();
            foreach (var p in usedParams)
            {
                datas.Add(p.Name, p.Data);
            }

            return datas;
        }

        internal static string ApplyParams(string sql, IEnumerable<ParamInfo> usedParams)
        {
            return sql;
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

            var gobalValue = data as IGobalValue;
            if (gobalValue != null && !queryParams.TryGetValue(key, out data))
            {
                data = gobalValue.GetValue(queryParams, realKey);
                queryParams.Add(key, data);
            }

            return new ParamInfo
            {
                Type = ParamType.Global,
                Name = key,
                Data = data
            };
        }
    }
}
