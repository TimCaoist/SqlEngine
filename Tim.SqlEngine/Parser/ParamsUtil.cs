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
        public static IEnumerable<ParamInfo> GetParams(Context context, System.Text.RegularExpressions.MatchCollection matches)
        {
            ICollection<ParamInfo> paramInfos = new List<ParamInfo>();
            foreach (Match match in matches)
            {
                var paramInfo = GetParamData(context, match.ToString().TrimEnd(')', ' '));
                paramInfo.Match = match;
                paramInfos.Add(paramInfo);
            }

            return paramInfos;
        }

        public static ParamInfo GetParamData(Context context, string dataStr)
        {
            var paramHandler = ParamHandlerFactory.Find(dataStr);
            return paramHandler.GetParamInfo(context, dataStr);
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
