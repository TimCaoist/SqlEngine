using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Tim.SqlEngine.Models;
using Tim.SqlEngine.Parser.ParamHandler;
using Tim.SqlEngine.PlugIn;

namespace Tim.SqlEngine.Parser
{
    public static class ParamsUtil
    {
        private const string ParamStr = "@.*?[, ]|@.*?[)]";

        private readonly static char[] ParamEndChar = new char[] { '_', ')', '(', ' ', SqlKeyWorld.Split, SqlKeyWorld.Split3 };

        public static Tuple<IEnumerable<ParamInfo>, MatchCollection> GetParams(IContext context, string eval)
        {
            var matches = Regex.Matches(eval, ParamStr);
            if (matches.Count == 0)
            {
                return Tuple.Create(Enumerable.Empty<ParamInfo>(), matches);
            }

            ICollection<ParamInfo> paramInfos = new List<ParamInfo>();
            foreach (Match match in matches)
            {
                var paramInfo = GetParamData(context, match.ToString().TrimEnd(ParamEndChar));
                if (paramInfo == null)
                {
                    continue;
                }

                paramInfo.Match = match;
                paramInfos.Add(paramInfo);
            }

            return Tuple.Create<IEnumerable<ParamInfo>, MatchCollection>(paramInfos, matches);
        }

        public static string ApplyParams(string str, IEnumerable<ParamInfo> paramInfos, System.Text.RegularExpressions.MatchCollection matches)
        {
            if (matches.Count == 0)
            {
                return str;
            }

            var total = matches.Count;
            for (var i = total - 1; i >= 0; i--)
            {
                var seg = matches[i];
                var startIndex = seg.Index;
                var endIndex = seg.Index + seg.Length;
                str = str.Remove(startIndex, endIndex - startIndex);
                str = str.Insert(startIndex, paramInfos.ElementAt(i).Data.ToString());
            }

            return str;
        }

        public static ParamInfo GetParamData(IContext context, string dataStr)
        {
            var paramHandler = ParamHandlerFactory.Find(dataStr);
            return paramHandler.GetParamInfo(context, dataStr);
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
