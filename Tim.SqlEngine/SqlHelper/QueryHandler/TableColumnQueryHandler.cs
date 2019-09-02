using System;
using System.Collections.Generic;
using System.Linq;
using Tim.SqlEngine.Models;
using Tim.SqlEngine.ValueSetter;

namespace Tim.SqlEngine.SqlHelper.QueryHandler
{
    public class TableColumnQueryHandler : SimpleQueryHandler
    {
        public override int Type => 5;

        private readonly static string DBFormatter = "sql_engine_db_{0}_columns";

        private readonly static string DBName = "db_name";

        private readonly static string Catalog = "Catalog=";

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

            var connStr = SqlEnginerConfig.GetConnection(queryConfig.Connection);
            var startIndex = connStr.IndexOf(Catalog, StringComparison.OrdinalIgnoreCase);
            var endIndex = connStr.IndexOf(";" , startIndex);
            var dbName = connStr.Substring(startIndex + 8, endIndex - startIndex - 8);
            queryConfig.CacheConfig = new CacheUtil.Models.CacheConfig
            {
                Key = string.Format(DBFormatter, dbName)
            }; 

            queryConfig.Config[DBName] = dbName;
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
            }).ToDatas<Column>().Where(c => c.TableName == config.Table).ToArray();

            return columns;
        }

        protected override object DoQuery(Context context)
        {
            var queryConfig = context.Config;
            var dbName = queryConfig.Config[DBName].ToString();
            context.Config.Sql = $"SELECT TABLE_SCHEMA as DBName, TABLE_NAME as TableName, COLUMN_NAME as ColName, COLUMN_TYPE as DataType, IS_NULLABLE as AllowNull, COLUMN_COMMENT as Comment, COLUMN_DEFAULT as DefaultValue, Extra, Column_key as \'Key\' FROM information_schema.columns where table_schema = '{dbName}'";
            return base.Query(context);
        }
    }
}
