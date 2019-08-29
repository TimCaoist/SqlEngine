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

            if (cols.ContainsKey(Id))
            {
                sql.Append(string.Concat(Id, " = @", cols[Id]));
            }
            else if (cols.ContainsKey("id"))
            {
                sql.Append(string.Concat(Id, " = @", cols["id"]));
            }
            else
            {
                sql.Append(string.Concat(Id, " = @id"));
            }

            return sql.ToString();
        }
    }
}
