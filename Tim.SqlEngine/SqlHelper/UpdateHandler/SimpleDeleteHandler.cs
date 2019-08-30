using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tim.SqlEngine.Models;

namespace Tim.SqlEngine.SqlHelper.UpdateHandler
{
    public class SimpleDeleteHandler : BaseSimpleUpdateHandler
    {
        public override int Type => 3;

        public override string BuilderSql(UpdateContext updateContext, UpdateConfig config, IDictionary<string, string> cols)
        {
            var sql = new StringBuilder();
            sql.Append($"delete from {config.Table} where ");

            if (!string.IsNullOrEmpty(config.Filter))
            {
                sql.Append(config.Filter);
                return sql.ToString();
            }

            var key = GetKeyName(config, cols);
            sql.Append(string.Concat(SqlKeyWorld.Id, " = @", cols[key]));
            return sql.ToString();
        }

        protected override void ApplyRules(UpdateContext updateContext, UpdateConfig config, IDictionary<string, string> cols)
        {
        }
    }
}
