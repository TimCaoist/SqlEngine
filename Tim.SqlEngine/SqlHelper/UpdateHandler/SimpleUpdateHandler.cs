using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tim.SqlEngine.Models;

namespace Tim.SqlEngine.SqlHelper.UpdateHandler
{
    public class SimpleUpdateHandler : BaseSimpleUpdateHandler
    {
        public override int Type => 2;

        public override string BuilderSql(UpdateContext updateContext, UpdateConfig config, IDictionary<string, string> cols)
        {
            var sql = new StringBuilder();
            sql.Append($"update {config.Table} set ");

            var cCount = cols.Count();
            for (var i = 0; i < cCount; i++)
            {
                var col = cols.ElementAt(i);
                if (col.Key.Equals("id", StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                sql.Append($" {col.Key}=@{col.Value}");
                if (i != cCount - 1)
                {
                    sql.Append(SqlKeyWorld.Split3);
                }
            }

            if (!string.IsNullOrEmpty(config.Filter))
            {
                sql.Append(string.Concat(" where ", config.Filter));
                return sql.ToString();
            }

            if (cols.ContainsKey(Id))
            {
                sql.Append(string.Concat(" where ", Id, " = @", cols[Id]));
            }
            else if (cols.ContainsKey("id"))
            {
                sql.Append(string.Concat(" where ", Id, " = @", cols["id"]));
            }
            else
            {
                sql.Append(string.Concat(" where ", Id, " = @id"));
            }

            return sql.ToString();
        }
    }
}
