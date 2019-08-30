using Newtonsoft.Json.Linq;
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
            return ToSingleData<TData>(obj, default(TData));
        }

        public static TData ToSingleData<TData>(this object obj, TData defaultData)
        {
            if (obj == null)
            {
                return defaultData;
            }

            JValue val = obj as JValue;
            if (val != null)
            {
                return val.ToObject<TData>();
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
