using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tim.SqlEngine.Models;
using Tim.SqlEngine.SqlHelper.QueryHandler;

namespace Tim.SqlEngine.SqlHelper.UpdateHandler
{
    public abstract class BaseSimpleUpdateHandler : BaseUpdateHandler
    {
        protected readonly static string Id = "Id";

        public override object Update(UpdateContext context)
        {
            var config = context.Config;
            if (string.IsNullOrEmpty(config.Connection))
            {
                config.Connection = context.HandlerConfig.Connection;
            }

            var sql = config.Sql;
            if (!string.IsNullOrEmpty(sql))
            {
                return SqlExcuter.ExcuteScalar(context);
            }

            if (string.IsNullOrEmpty(config.Table))
            {
                throw new ArgumentException("config.Table");
            }

            IDictionary<string, string> cols = new Dictionary<string, string>();
            var fields = config.Fields;
            if (config.Fields == null)
            {
                fields = TableColumnQueryHandler.QueryColumns(config);
            }

            foreach (var field in fields)
            {
                if (field.IndexOf(SqlKeyWorld.Split) <= 0)
                {
                    cols.Add(field, field);
                    continue;
                }

                var fArray = field.Split(SqlKeyWorld.Split);
                cols.Add(fArray[0], fArray[1]);
            }

            config.Sql = BuilderSql(context, config, cols);
            if (config.InTran == false)
            {
                return SqlExcuter.Excute(context);
            }

            return SqlExcuter.ExcuteTrann(context);
        }

        public abstract string BuilderSql(UpdateContext updateContext, UpdateConfig config, IDictionary<string, string> cols);
    }
}
