using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tim.SqlEngine.Common
{
    public static class DictHelper
    {
        public static void ReplaceOrInsert<TKey, TData>(this IDictionary<TKey, TData> datas, TKey key, TData data)
        {
            if (datas.ContainsKey(key))
            {
                datas[key] = data;
                return;
            }

            datas.Add(key, data);
        }
    }
}
