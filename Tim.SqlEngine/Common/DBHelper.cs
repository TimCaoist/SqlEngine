using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tim.SqlEngine.Models;
using Tim.SqlEngine.ValueSetter;

namespace Tim.SqlEngine.Common
{
    public static class DBHelper
    {
        private const char Split = '\'';

        public readonly static string Where = " WHERE ";

        private readonly static string Equla = "=@";

        public readonly static string UpdateFormatter = "UPDATE {0} SET ";

        public static string GetDBValue(object data, IEnumerable<Column> columns)
        {
            if (columns.Any() == false)
            {
                return string.Concat(Split, data.ToString(), Split);
            }

            return string.Concat(Split, data.ToString(), Split);
        }

        public static bool SpecailColumn(Column column)
        {
            return false;
        }

        public static string BuildColVal(KeyValuePair<string, string> col, IValueSetter valueSetter, object data, IEnumerable<Column> columns)
        {
            var v = col.Value;
            if (v.StartsWith(SqlKeyWorld.ParamStart))
            {
                return v;
            }

            var val = valueSetter.GetValue(data, v);
            return DBHelper.GetDBValue(val, columns);
        }

        public static string BuildInsertSql(IDictionary<string, string> cols, string table)
        {
            var colStrs = cols.Values.Select(v => {
                if (v.StartsWith(SqlKeyWorld.ParamStart))
                {
                    return v;
                }

                return string.Concat(SqlKeyWorld.ComplexDataObjectStart, v, SqlKeyWorld.WhiteSpace);
            });

            return $"INSERT INTO {table} ({string.Join(SqlKeyWorld.Split1, cols.Keys)}) VALUES ({string.Join(SqlKeyWorld.Split1, colStrs)})";
        }

        public static string BuildUpdateSql(IDictionary<string, string> cols, UpdateConfig config, string key)
        {
            var sql = new StringBuilder();
            sql.AppendFormat(UpdateFormatter, config.Table);
            var cCount = cols.Count();
            for (var i = 0; i < cCount; i++)
            {
                var col = cols.ElementAt(i);
                if (col.Key.Equals(key, StringComparison.OrdinalIgnoreCase))
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

            sql.Append(string.Concat(Where, key, Equla, cols[key]));
            return sql.ToString();
        }
    }
}
