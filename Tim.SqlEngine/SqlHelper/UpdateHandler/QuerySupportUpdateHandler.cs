using Tim.SqlEngine.Common;
using Tim.SqlEngine.Models;

namespace Tim.SqlEngine.SqlHelper.UpdateHandler
{
    /// <summary>
    /// 用户在Update中支持查询的类，类似代理
    /// </summary>
    public class QuerySupportUpdateHandler : BaseUpdateHandler
    {
        public override int Type => 8;

        private const string QueryName = "query_name";

        public override object Update(UpdateContext context)
        {
            var config = context.Config;
            return SqlEnginer.Query(config.Config[QueryName].ToSingleData<string>(), context.ComplexData, context.Params);
        }
    }
}
