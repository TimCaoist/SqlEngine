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
        internal static IEnumerable<object> ExcuteQuery(IContext context, IValueSetter valueSetter, string querySql = "")
        {
            return Excute<IEnumerable<object>>(context, (cmd) =>
            {
                using (var dataReader = cmd.ExecuteReader())
                {
                    var columns = GetColumns(dataReader);
                    return valueSetter.SetterDatas(context.GetConfig(), dataReader, columns);
                }
            }, querySql);
        }

        internal static object ExcuteScalar(IContext context)
        {
            return Excute(context, (cmd) =>
            {
                return cmd.ExecuteScalar();
            });
        }

        public static TObject Excute<TObject>(IContext context, Func<MySqlCommand, TObject> doExcute, string querySql = "")
        {
            var queryConfig = context.GetConfig();
            using (var connection = new MySqlConnection(SqlEnginerConfig.GetConnection(queryConfig.Connection)))
            {
                connection.Open();
                var realSql = SqlParser.Convert(context, string.IsNullOrEmpty(querySql) ? queryConfig.Sql : querySql);
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
                var result = cmd.ExecuteNonQuery();
                return GetResult(context, cmd, result);
            });
        }

        private static object GetResult(UpdateContext context, MySqlCommand cmd, object result)
        {
            var queryConfig = context.Config;
            if (queryConfig.ReturnId == false)
            {
                return result;
            }

            var id = cmd.LastInsertedId;
            if (id != 0)
            {
                return id;
            }

            object data;
            if (context.ContentParams.TryGetValue(SqlKeyWorld.ReturnKey, out data))
            {
                return data;
            }

            return result;
        }

        public static object ExcuteTrann(UpdateContext context)
        {
            if (context.ExcuteFail == true)
            {
                return false;
            }

            var config = context.Config;
            var conn = context.OpenTran(config.Connection);
            var connection = conn.Item1; 
            var trann = conn.Item2;

            var realSql = SqlParser.Convert(context, config.Sql);
            var cmd = connection.CreateCommand();
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
                context.AddCmd(cmd);
                return GetResult(context, cmd, result);
            }
            catch (Exception ex)
            {
                context.RollBack();
                throw;
            }
        }
    }
}
