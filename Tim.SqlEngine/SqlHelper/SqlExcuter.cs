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
    internal static class SqlExcuter
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
        internal static IEnumerable<object> ExcuteQuery(IContext context, IValueSetter valueSetter)
        {
            return Excute<IEnumerable<object>>(context, (cmd) =>
            {
                using (var dataReader = cmd.ExecuteReader())
                {
                    var columns = GetColumns(dataReader);
                    return valueSetter.SetterDatas((QueryConfig)context.GetConfig(), dataReader, columns);
                }
            });
        }

        internal static object ExcuteScalar(IContext context)
        {
            return Excute(context, (cmd) =>
            {
                return cmd.ExecuteScalar();
            });
        }

        public static TObject Excute<TObject>(IContext context, Func<MySqlCommand, TObject> doExcute)
        {
            var queryConfig = context.GetConfig();
            using (var connection = new MySqlConnection(SqlEnginerConfig.GetConnection(queryConfig.Connection)))
            {
                connection.Open();
                var realSql = SqlParser.Convert(context, queryConfig.Sql);
                using (var cmd = connection.CreateCommand())
                {
                    cmd.CommandText = realSql.Item1;
                    if (realSql.Item2 == null || !realSql.Item2.Any())
                    {
                        return doExcute(cmd);
                    }

                    foreach (var ps in realSql.Item2)
                    {
                        cmd.Parameters.AddWithValue(ps.Key, ps.Value);
                    }

                    return doExcute(cmd);
                }
            }
        }

        internal static object Excute(UpdateContext context)
        {
            return Excute(context, (cmd) =>
            {
                var queryConfig = context.Config;
                var result = cmd.ExecuteNonQuery();
                if (queryConfig.ReturnId)
                {
                    return cmd.LastInsertedId;
                }

                return result;
            });
        }

        public static object ExcuteTrann(UpdateContext context)
        {
            if (context.ExcuteFail == true)
            {
                return false;
            }

            var queryConfig = context.Config;
            var conn = context.OpenTran(queryConfig.Connection);
            var connection = conn.Item1; 
            var trann = conn.Item2;

            var realSql = SqlParser.Convert(context, queryConfig.Sql);
            using (var cmd = connection.CreateCommand())
            {
                cmd.CommandText = realSql.Item1;
                cmd.Transaction = trann;
                try
                {
                    if (realSql.Item2 != null && realSql.Item2.Any())
                    {
                        foreach (var ps in realSql.Item2)
                        {
                            cmd.Parameters.AddWithValue(ps.Key, ps.Value);
                        }
                    }

                    var result = cmd.ExecuteNonQuery();
                    if (queryConfig.ReturnId)
                    {
                        return cmd.LastInsertedId;
                    }

                    return result;
                }
                catch(Exception ex) {
                    context.RollBack();
                    throw;
                }
            }
        }
    }
}
