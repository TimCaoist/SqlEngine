using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tim.SqlEngine.Common;
using Tim.SqlEngine.Models;
using Tim.SqlEngine.ValueSetter;

namespace Tim.SqlEngine.SqlHelper.QueryHandler
{
    public class TableColumnQueryHandler : SimpleQueryHandler
    {
        public override int Type => 5;

        private readonly static IDictionary<string, IEnumerable<Column>> Columns = new Dictionary<string, IEnumerable<Column>>();

        private readonly static object sync = new object();

        private readonly static string tableName = "tableName";

        private readonly static string columnType = "Tim.SqlEngine,Tim.SqlEngine.Models.Column";

        public static IEnumerable<Column> QueryColumns(UpdateConfig config)
        {
            if (string.IsNullOrEmpty(config.Table))
            {
                return Enumerable.Empty<Column>();
            }

            var queryHandler = QueryHandlerFactory.GetQueryHandler(5);
            var queryConfig = new QueryConfig
            {
                Config = new Newtonsoft.Json.Linq.JObject(),
                Connection = config.Connection
            };

            queryConfig.Config[tableName] = config.Table;
            queryConfig.Config[ValueSetterCreater.TypeStr] = columnType;
            var columns = queryHandler.Query(new Context
            {
                HandlerConfig = new HandlerConfig
                {
                    Connection = config.Connection,
                },
                Configs = new QueryConfig[] {
                     queryConfig,
                }
            }).ToDatas<Column>().ToArray();

            return columns;
        }

        public override object Query(Context context)
        {
            var queryConfig = context.Config;
            var connStr = SqlEnginerConfig.GetConnection(queryConfig.Connection);
            var startIndex = connStr.IndexOf("Catalog=", StringComparison.OrdinalIgnoreCase);
            var endIndex = connStr.IndexOf(";", startIndex);
            var dbName = connStr.Substring(startIndex + 8, endIndex - startIndex - 8);
            var key = string.Concat(dbName);
            var table = queryConfig.Config[tableName].ToString();
            IEnumerable<Column> columns;
            if (Columns.TryGetValue(key, out columns))
            {
                return columns.Where(c => c.TableName == table).ToArray();
            }

            context.Config.Sql = $"SELECT TABLE_SCHEMA as DBName, TABLE_NAME as TableName, COLUMN_NAME as ColName, COLUMN_TYPE as DataType, IS_NULLABLE as AllowNull, COLUMN_COMMENT as Comment, COLUMN_DEFAULT as DefaultValue, Extra, Column_key as \'Key\' FROM information_schema.columns where table_schema = '{dbName}'";
            columns = base.Query(context).ToDatas<Column>();
            if (!Columns.ContainsKey(key))
            {
                lock (sync)
                {
                    if (!Columns.ContainsKey(key))
                    {
                        Columns.Add(key, columns);
                    }
                }
            }

            return columns.Where(c => c.TableName == table).ToArray();
        }
    }
}
