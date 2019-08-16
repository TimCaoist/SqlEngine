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
        public ParamInfo GetParamInfo(Context context, string dataStr)
        {
            var key = dataStr.Replace(SqlKeyWorld.ParamStart, string.Empty);
            object data;
            if (!context.QueryParams.TryGetValue(key, out data))
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
