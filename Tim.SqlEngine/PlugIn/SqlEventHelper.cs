using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tim.SqlEngine.Models;

namespace Tim.SqlEngine.PlugIn
{
    public static class SqlEventHelper
    {
        private const string Query_End_Call = "query_end_call";
        public static object OnQueryEnd(this BaseHadlerConfig config, object data, IDictionary<string, object> queryParams)
        {
            if (config.Config == null || config.Config[Query_End_Call] == null)
            {
                return data;
            }

            var typeStrs = config.Config[Query_End_Call].ToString().Split(SqlKeyWorld.Split3);
            if (typeStrs.Length != 2)
            {
                throw new ArgumentException("query_end_call配置错误");
            }

            var queryEnd = (IQueryEnd)ReflectUtil.ReflectUtil.CreateInstance(typeStrs[0], typeStrs[1]);
            return queryEnd.Handler(config, data, queryParams);
        }
    }
}
