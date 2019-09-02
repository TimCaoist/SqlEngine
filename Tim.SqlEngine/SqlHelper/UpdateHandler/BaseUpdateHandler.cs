using System.Collections.Generic;
using System.Linq;
using Tim.CacheUtil.Models;
using Tim.SqlEngine.Models;
using Tim.SqlEngine.SqlHelper.QueryHandler;

namespace Tim.SqlEngine.SqlHelper.UpdateHandler
{
    public abstract class BaseUpdateHandler : IUpdateHandler
    {
        public abstract int Type { get; }

        protected abstract object DoUpdate(UpdateContext context);

        protected virtual CacheConfig GetCacheConfig(UpdateContext context)
        {
            var queryConfig = context.Config;
            return queryConfig.CacheConfig;
        }

        public virtual object Update(UpdateContext context)
        {
            var result = DoUpdate(context);
            var cacheConfig = GetCacheConfig(context);
            if (cacheConfig != null)
            {
                var cacheProxy = CacheUtil.CacheUtil.Create();
                cacheProxy.Remove(cacheConfig, context);
            }

            return result;
        }

        protected readonly static string LowerId = SqlKeyWorld.Id.ToLower();
             
        protected static string GetKeyName(UpdateConfig config, IDictionary<string, string> cols)
        {
            var key = config.Key;
            if (!string.IsNullOrEmpty(key))
            {
                return key;
            }

            if (cols.ContainsKey(SqlKeyWorld.Id))
            {
                return SqlKeyWorld.Id;
            }

            if (cols.ContainsKey(LowerId))
            {
                return LowerId;
            }

            var keyColumn = TableColumnQueryHandler.QueryColumns(config).FirstOrDefault(c => c.IsKey);
            if (keyColumn != null)
            {
                return keyColumn.ColName;
            }

            return string.Empty;
        }

        protected IDictionary<string, string> GetCols(UpdateConfig config)
        {
            IDictionary<string, string> cols = new Dictionary<string, string>();
            var fields = config.Fields;
            if (config.Fields == null)
            {
                fields = TableColumnQueryHandler.QueryColumns(config).Select(c => c.ColName).ToArray();
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

            return cols;
        }
    }
}
