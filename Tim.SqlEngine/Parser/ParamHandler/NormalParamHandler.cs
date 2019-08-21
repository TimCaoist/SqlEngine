using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tim.SqlEngine.Models;

namespace Tim.SqlEngine.Parser.ParamHandler
{
    public class NormalParamHandler : IParamHandler
    {
        private readonly static string[] IngoresKeys = new string[] { "IDENTITY" };

        public ParamInfo GetParamInfo(IContext context, string dataStr)
        {
            var key = dataStr.Replace(SqlKeyWorld.ParamStart, string.Empty);
            if (IngoresKeys.Any(k => k.Equals(key, StringComparison.OrdinalIgnoreCase)))
            {
                return null;
            }

            object data;
            if (!context.Params.TryGetValue(key, out data))
            {
                throw new ArgumentNullException(string.Concat("参数", key, "不存在!"));
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
            return paramStr.StartsWith(SqlKeyWorld.ParamStart);
        }
    }
}
