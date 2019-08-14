using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Tim.SqlEngine.Models;

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
            if (!dataStr.StartsWith("@"))
            {
                return new ParamInfo
                {
                    Type = ParamType.Constant,
                    Name = string.Empty,
                    Data = dataStr
                };
            }

            var key = dataStr.Replace("@", string.Empty);
            if (key.StartsWith("g_", StringComparison.OrdinalIgnoreCase))
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

            return null;
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
            return new ParamInfo
            {
                Type = ParamType.Global,
                Name = key,
                Data = SqlEnginerConfig.GetGlobalDatas(keyArray[1])
            };
        }
    }
}
