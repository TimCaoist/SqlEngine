using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tim.SqlEngine.Models;
using Tim.SqlEngine.PlugIn;

namespace Tim.SqlEngine.ValueSetter
{
    public static class UpdateTrigger
    {
        public static void TriggeDefaultValues(UpdateContext updateContext, UpdateConfig config, IDictionary<string, string> cols)
        {
            var updateParams = updateContext.Params;
            var valueKeys = updateParams.Keys;
            ICollection<string> exceptKeys = new List<string>();
            foreach (var col in cols)
            {
                if (valueKeys.Contains(col.Value))
                {
                    continue;
                }

                exceptKeys.Add(col.Key);
            }

            if (exceptKeys.Any() == false)
            {
                return;
            }

            var rules =  SqlEnginerConfig.GetMatchRules(config.Connection, config.Table).OrderBy(r => r.RangeType).ToArray();
            foreach (var key in exceptKeys)
            {
                foreach (var rule in rules)
                {
                    var column = rule.Columns.FirstOrDefault(c => c.Name.Equals(key, StringComparison.OrdinalIgnoreCase));
                    if (column == null)
                    {
                        continue;
                    }

                    updateParams.Add(cols[key], GetColumnValue(config.Connection, config.Table, column, updateParams));
                    break;
                }
            }
        }

        public static object GetColumnValue(string dbKey, string tableName, ColumnRule columnRule, object paramData)
        {
            switch (columnRule.ValueType)
            {
                case ColumnValueType.Interface:
                    {
                        var typeStrs = columnRule.Value.ToString().Split(SqlKeyWorld.Split3);
                        var valueGetter = (IUpdateValueGetter)ReflectUtil.ReflectUtil.CreateInstance(typeStrs[0], typeStrs[1]);
                        return valueGetter.Get(dbKey, tableName, columnRule, paramData);
                    }
                case ColumnValueType.Func:
                    {
                        var func = (Func<string, string, ColumnRule, object, object>)columnRule.Value;
                        return func.Invoke(dbKey, tableName, columnRule, paramData);
                    }
                default:
                    return columnRule.Value;
            }
        }
    }
}
