using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tim.SqlEngine.Models;
using Tim.SqlEngine.ValueSetter;

namespace Tim.SqlEngine.SqlHelper.UpdateHandler
{
    public class SimpleUpdateHandler : BaseSimpleUpdateHandler
    {
        public override int Type => 2;

        private readonly static string Where = " where ";

        private readonly static string Equla = "=@";

        public override string BuilderSql(UpdateContext updateContext, UpdateConfig config, IDictionary<string, string> cols)
        {
            UpdateTrigger.TriggeValuesChecked(updateContext, config, cols, ActionType.Insert);

            var sql = new StringBuilder();
            sql.Append($"update {config.Table} set ");

            var cCount = cols.Count();
            for (var i = 0; i < cCount; i++)
            {
                var col = cols.ElementAt(i);
                if (col.Key.Equals(Id, StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                sql.Append($" {col.Key}{Equla}{col.Value}");
                if (i != cCount - 1)
                {
                    sql.Append(SqlKeyWorld.Split3);
                }
            }

            if (!string.IsNullOrEmpty(config.Filter))
            {
                sql.Append(string.Concat(Where, config.Filter));
                return sql.ToString();
            }

            var updateParams = updateContext.Params;
            if (updateParams.ContainsKey(Id))
            {
                sql.Append(string.Concat(Where, Id, Equla, Id));
            }
            else if (cols.ContainsKey(Id))
            {
                sql.Append(string.Concat(Where, Id, Equla, cols[Id]));
            }
            else
            {
                sql.Append(string.Concat(Where, Id, Equla, LowerId));
            }

            return sql.ToString();
        }
    }
}
