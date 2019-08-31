using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Tim.LambdaEngine;
using Tim.SqlEngine.Models;
using Tim.SqlEngine.Parser;
using Tim.SqlEngine.PlugIn;
using Tim.SqlEngine.ValueSetter.ValueChecked;

namespace Tim.SqlEngine.ValueSetter
{
    public static class UpdateTrigger
    {
        public static void TriggeValuesChecked(UpdateContext updateContext, object data, UpdateConfig config, IDictionary<string, string> cols, ActionType actionType, IValueSetter valueSetter, IEnumerable<string> keys)
        {
            var rules = SqlEnginerConfig.GetMatchRules(config.Connection, config.Table, actionType, UpdateType.CheckValue).OrderBy(r => r.RangeType).ToArray();
            if (rules.Any() == false)
            {
                return;
            }

            var columns = rules.SelectMany(r => r.Columns).ToArray();
            foreach (var key in keys)
            {
                if (!cols.Any(c => c.Value == key) || key.StartsWith(SqlKeyWorld.ParamStart))
                {
                    continue;
                }

                var realKey = cols.First(c => c.Value == key).Key;
                var matchColumns = columns.Where(c => c.Name == realKey).ToArray();
                if (matchColumns.Any() == false)
                {
                    continue;
                }

                foreach (var mc in matchColumns)
                {
                    IValueChecked valueChecked = ValueCheckedFactory.Create(mc.ValueType);
                    if (valueChecked == null || mc.Value == null)
                    {
                        continue;
                    }

                    var value = valueSetter.GetValue(data, key);
                    if (valueChecked.Checked(updateContext, mc, value, key, realKey))
                    {
                        continue;
                    }

                    var msg = mc.Name;
                    if (!string.IsNullOrEmpty(mc.Error))
                    {
                        msg = mc.Error;
                    }

                    throw new ArgumentException(msg);
                }
            }
        }

        public static void TriggeDefaultValues(UpdateContext updateContext, object data, UpdateConfig config, IDictionary<string, string> cols, IValueSetter valueSetter, IEnumerable<string> keys = null)
        {
            var valueKeys = keys;
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

            var rules = SqlEnginerConfig.GetMatchRules(config.Connection, config.Table, ActionType.Insert).OrderBy(r => r.RangeType).ToArray();
            foreach (var key in exceptKeys)
            {
                if (key.StartsWith(SqlKeyWorld.ParamStart))
                {
                    continue;
                }

                foreach (var rule in rules)
                {
                    var column = rule.Columns.FirstOrDefault(c => c.Name.Equals(key, StringComparison.OrdinalIgnoreCase));
                    if (column == null)
                    {
                        continue;
                    }

                    var colValue = GetColumnValue(config.Connection, config.Table, column, data);
                    valueSetter.SetField(cols[key], colValue);
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
