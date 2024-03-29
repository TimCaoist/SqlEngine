﻿using Tim.CacheUtil.Models;
using Tim.SqlEngine.Models;
using Tim.SqlEngine.Parser;

namespace Tim.SqlEngine.PlugIn
{
    internal class CacheNameCreator : CacheUtil.DefaultCacheKeyCreator
    {
        public override string GetKey(CacheConfig config, object state)
        {
            IContext context = (IContext)state;
            var prefix = GetRealText(config.KeyPrefix, context);
            var key = GetRealText(config.Key, context);
            return BuildKey(prefix, key);
        }

        public string GetRealText(string str, IContext context)
        {
            if (string.IsNullOrEmpty(str))
            {
                return string.Empty;
            }

            var paramInfos = ParamsUtil.GetParams(context, str);
            return ParamsUtil.ApplyParams(str, paramInfos.Item1, paramInfos.Item2);
        }
    }
}
