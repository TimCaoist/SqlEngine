using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Tim.SqlEngine.Models;
using Tim.SqlEngine.Parser;
using Tim.SqlEngine.ValueSetter;

namespace Tim.SqlEngine.SqlHelper
{
    internal static class SqlQueryExcuter
    {
        internal static IEnumerable<string> GetColumns(MySqlDataReader mySqlDataReader)
        {
            var filedCount = mySqlDataReader.FieldCount;
            ICollection<string> columns = new List<string>();
            for (var i = 0; i < filedCount; i++)
            {
                columns.Add(mySqlDataReader.GetName(i));
            }

            return columns;
        }
        internal static IEnumerable<object> ExcuteQuery(QueryConfig queryConfig, IValueSetter valueSetter, IDictionary<string, object> queryParam)
        {
            using (var connection = new MySqlConnection(SqlEnginerConfig.GetConnection(queryConfig.Connection)))
            {
                connection.Open();
                var realSql = SqlParser.Convert(queryConfig.Sql, queryParam);
                using (var cmd = new MySqlCommand(realSql.Item1, connection))
                {
                    if (realSql.Item2 != null && realSql.Item2.Any())
                    {
                        foreach (var ps in realSql.Item2)
                        {
                            cmd.Parameters.AddWithValue(ps.Key, ps.Value);
                        }
                    }

                    using(var dataReader = cmd.ExecuteReader())
                    {
                        var columns = GetColumns(dataReader);
                        return valueSetter.SetterDatas(queryConfig, dataReader, columns);
                    }
                }
            }
        }

        internal static object ExcuteScalar(QueryConfig queryConfig, IDictionary<string, object> queryParam)
        {
            using (var connection = new MySqlConnection(SqlEnginerConfig.GetConnection(queryConfig.Connection)))
            {
                connection.Open();
                var realSql = SqlParser.Convert(queryConfig.Sql, queryParam);
                using (var cmd = new MySqlCommand(realSql.Item1, connection))
                {
                    if (realSql.Item2 != null && realSql.Item2.Any())
                    {
                        foreach (var ps in realSql.Item2)
                        {
                            cmd.Parameters.AddWithValue(ps.Key, ps.Value);
                        }
                    }

                    return cmd.ExecuteScalar();
                }
            }
        }
    }
}
