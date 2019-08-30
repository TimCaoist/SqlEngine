using System;
using System.Collections.Generic;
using Tim.SqlEngine.Models;
using Tim.SqlEngine.SqlHelper.QueryHandler;

namespace Tim.SqlEngine.SqlHelper.UpdateHandler
{
    public abstract class BaseSimpleUpdateHandler : BaseUpdateHandler
    {
        protected abstract void ApplyRules(UpdateContext updateContext, UpdateConfig config, IDictionary<string, string> cols);


        public override object Update(UpdateContext context)
        {
            var config = context.Config;
            if (string.IsNullOrEmpty(config.Connection))
            {
                config.Connection = context.HandlerConfig.Connection;
            }

            var cols = GetCols(config);
            ApplyRules(context, config, cols);
            var sql = config.Sql;
            if (!string.IsNullOrEmpty(sql))
            {
                return SqlExcuter.ExcuteScalar(context);
            }

            if (string.IsNullOrEmpty(config.Table))
            {
                throw new ArgumentException("config.Table");
            }

            config.Sql = BuilderSql(context, config, cols);
            object result;
            if (config.InTran == false)
            {
                result = SqlExcuter.Excute(context);
            }
            else {
                result = SqlExcuter.ExcuteTrann(context);
            }

            return result;
        }

        public abstract string BuilderSql(UpdateContext updateContext, UpdateConfig config, IDictionary<string, string> cols);
    }
}
