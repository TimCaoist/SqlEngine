using System;
using System.Collections.Generic;
using Tim.SqlEngine.Models;

namespace Tim.SqlEngine.Common
{
    public static class DictHelper
    {
        public static IDictionary<string, object> ParamsToDictionary(this IEnumerable<ParamInfo> paramInfos, bool original = false)
        {
            IDictionary<string, object> datas = new Dictionary<string, object>();
            foreach (var p in paramInfos)
            {
                if (datas.ContainsKey(p.Name))
                {
                    continue;
                }

                if (!original)
                {
                    datas.Add(p.Name, p.Data);
                }
                else {
                    datas.Add(p.Name, p.OriginalData);
                }
            }

            return datas;
        }

        public static void ReplaceOrInsert<TKey, TData>(this IDictionary<TKey, TData> datas, TKey key, TData data)
        {
            if (datas.ContainsKey(key))
            {
                datas[key] = data;
                return;
            }

            datas.Add(key, data);
        }

        public static TReturn CreateOrGet<TKey, TData, TReturn>(this IDictionary<TKey, TData> datas, TKey key, Func<TKey, TData> func) where TData :class where TReturn:class
        {
            TData outData;
            if (datas.TryGetValue(key, out outData))
            {
                return outData as TReturn;
            }

            outData = func.Invoke(key);
            datas.Add(key, outData);
            return outData as TReturn;
        }
    }
}
