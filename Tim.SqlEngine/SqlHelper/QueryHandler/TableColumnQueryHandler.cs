using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tim.SqlEngine.Models;

namespace Tim.SqlEngine.SqlHelper.QueryHandler
{
    public class TableColumnQueryHandler : SimpleRecordQueryHandler
    {
        public override int Type => 5;

        private readonly static IDictionary<string, object> Columns = new Dictionary<string, object>();

        private readonly static object Sync = new object();

        private readonly static string TableName = "tableName";

        public static IEnumerable<string> QueryColumns(UpdateConfig config)
        {
            var queryHandler = QueryHandlerFactory.GetQueryHandler(5);
            var queryConfig = new QueryConfig
            {
                Config = new Newtonsoft.Json.Linq.JObject(),
                Connection = config.Connection
            };

            queryConfig.Config["tableName"] = config.Table;
            var fields = queryHandler.Query(new Context
            {
                HandlerConfig = new HandlerConfig
                {
                    Connection = config.Connection,

                },
                Configs = new QueryConfig[] {
                           queryConfig,
                }
            }).ToDatas<string>().ToArray();

            return fields;
        }

        public override object Query(Context context)
        {
            var queryConfig = context.Config;
            var tableName = queryConfig.Config[TableName];
            var connStr = SqlEnginerConfig.GetConnection(queryConfig.Connection);
            var startIndex = connStr.IndexOf("Catalog=", StringComparison.OrdinalIgnoreCase);
            var endIndex = connStr.IndexOf(";", startIndex);
            var dbName = connStr.Substring(startIndex + 8, endIndex - startIndex - 8);
            var key = string.Concat(dbName, "_", tableName);
            object columns;
            if (Columns.TryGetValue(key, out columns))
            {
                return columns;
            }

            context.Config.Sql = $"select COLUMN_NAME from information_schema.COLUMNS where table_name = '{tableName}' and table_schema = '{dbName}'";
            columns = base.Query(context);
            if (!Columns.ContainsKey(key))
            {
                lock (Sync)
                {
                    if (!Columns.ContainsKey(key))
                    {
                        Columns.Add(key, columns);
                    }
                }
            }

            return columns;
        }
    }
}
