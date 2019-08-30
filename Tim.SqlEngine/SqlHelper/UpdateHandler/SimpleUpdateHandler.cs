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
            var sql = new StringBuilder();
            sql.Append($"update {config.Table} set ");

            var cCount = cols.Count();
            for (var i = 0; i < cCount; i++)
            {
                var col = cols.ElementAt(i);
                if (col.Key.Equals(SqlKeyWorld.Id, StringComparison.OrdinalIgnoreCase))
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

            var key = GetKeyName(config, cols);
            sql.Append(string.Concat(Where, SqlKeyWorld.Id, Equla, cols[SqlKeyWorld.Id]));
            return sql.ToString();
        }

        protected override void ApplyRules(UpdateContext updateContext, UpdateConfig config, IDictionary<string, string> cols)
        {
            var uParams = updateContext.Params;
            IValueSetter valueSetter = ValueSetterCreater.Create(uParams);
            UpdateTrigger.TriggeValuesChecked(updateContext, uParams, config, cols, ActionType.Update, valueSetter);
        }
    }
}
