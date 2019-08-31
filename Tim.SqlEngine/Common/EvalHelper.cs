using System;
using System.Collections.Generic;
using Tim.SqlEngine.Models;
using Tim.SqlEngine.Parser;
using System.Linq;
using System.Text.RegularExpressions;

namespace Tim.SqlEngine.Common
{
    public static class EvalHelper
    {
        private const string Data = "data";

        public static Delegate GetDelegate(IContext context, string eval, object data)
        {
            var matches = Regex.Matches(eval, string.Intern("@.*?[, ]"));
            var usedParams = ParamsUtil.GetParams(context, matches).ParamsToDictionary(true);
            var count = usedParams.Count;
            if (count > 0)
            {
                for (var i = count - 1; i >= 0; i--)
                {
                    var item = usedParams.ElementAt(i);
                    if (!item.Key.Contains(SqlKeyWorld.Spot))
                    {
                        continue;
                    }

                    usedParams.Remove(item.Key);
                    var newKey = item.Key.Replace(SqlKeyWorld.Spot, SqlKeyWorld.Underline);
                    eval = eval.Replace(item.Key, newKey);
                    usedParams.Add(newKey, item.Value);
                }
            }

            var paramTypes = new Dictionary<string, Type>();
            paramTypes.Add(Data, data.GetType());
            Delegate @delegate = LambdaEngine.ExpressionBuilder.Build(eval, usedParams, paramTypes);
            return @delegate;
        }
    }
}
