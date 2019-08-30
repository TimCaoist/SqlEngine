using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tim.SqlEngine.Common
{
    public static class DataConvert
    {
        public static TData ToSingleData<TData>(this object obj)
        {
            if (obj == null)
            {
                return default(TData);
            }

            return (TData)obj;
        }

        public static TData ToSingleData<TData>(this object obj, TData defaultData)
        {
            if (obj == null)
            {
                return defaultData;
            }

            return (TData)obj;
        }

        public static IEnumerable<TData> ToDatas<TData>(this object obj)
        {
            var datas = (IEnumerable<object>)obj;
            return datas.Cast<TData>();
        }
    }
}
