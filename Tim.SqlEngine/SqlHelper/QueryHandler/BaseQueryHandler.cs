using Tim.CacheUtil.Models;
using Tim.SqlEngine.Models;

namespace Tim.SqlEngine.SqlHelper.QueryHandler
{
    public abstract class BaseQueryHandler : IQueryHandler
    {
        public abstract int Type { get; }

        protected abstract object DoQuery(Context context);

        protected virtual CacheConfig GetCacheConfig(Context context)
        {
            var queryConfig = context.Config;
            return queryConfig.CacheConfig;
        }

        public object Query(Context context)
        {
            var cacheConfig = GetCacheConfig(context);
            if (cacheConfig == null)
            {
                return DoQuery(context);
            }

            var cacheProxy = CacheUtil.CacheUtil.Create();
            return cacheProxy.GetAndAddData(cacheConfig, () =>
            {
                return DoQuery(context);
            });
        }
    }
}
