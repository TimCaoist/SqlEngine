using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tim.SqlEngine.Models;

namespace Tim.SqlEngine.ValueSetter
{
    public static class ValueGetter
    {
        public static object GetValue(ReleatedQuery queryConfig, object data)
        {
            if (!queryConfig.IsSingle)
            {
                return data;
            }

            var type = data.GetType();
            var isArray = type.GetMethod("GetEnumerator") != null;
            if (isArray)
            {
                var datas = (IEnumerable<object>)data;
                return datas.FirstOrDefault();
            }

            return data;
        }

        public static IEnumerable<object> GetFilterValues(Dictionary<string, string> matcherFields, object parent, IEnumerable<object> datas)
        {
            ICollection<object> notMatchDatas = new List<object>();
            foreach (var mf in matcherFields)
            {
                var parentValue = ReflectUtil.ReflectUtil.GetProperty(parent, mf.Key);
                foreach (var data in datas)
                {
                    if (notMatchDatas.Contains(data))
                    {
                        continue;
                    }

                    var childValue = ReflectUtil.ReflectUtil.GetProperty(data, mf.Value);
                    if (!childValue.Equals(parentValue))
                    {
                        notMatchDatas.Add(data);
                    }
                }
            }

            return datas.Except(notMatchDatas).ToArray();
        }

        public static ValueInfo GetValue(string field, object data)
        {
            var type = data.GetType();
            var isArray = type.GetMethod("GetEnumerator") != null;
            
            if (!isArray)
            {
                return new ValueInfo
                {
                    Data = ReflectUtil.ReflectUtil.GetProperty(data, field),
                    IsArray = false
                };
            }

            ICollection<object> outDatas = new List<object>();
            var datas = (IEnumerable<object>)data;
            foreach (var item in datas)
            {
                outDatas.Add(ReflectUtil.ReflectUtil.GetProperty(item, field));
            }

            return new ValueInfo
            {
                Data = outDatas,
                IsArray = true
            };
        }

        public static ValueInfo GetValue(IEnumerable<string> fields, object data)
        {
            var fCount = fields.Count();
            var fetchData = data;
            ValueInfo valueInfo = null;
            for (var i = 0; i < fCount; i++)
            {
                valueInfo = GetValue(fields.ElementAt(i), fetchData);
                fetchData = valueInfo.Data;
            }

            return valueInfo;
        }

        public static string Builder(ValueInfo valueInfo)
        {
            if (valueInfo.IsArray == false)
            {
                return valueInfo.Data.ToString();
            }

            var datas = (IEnumerable<object>)valueInfo.Data;
            if (datas.Any() == false)
            {
                return string.Empty;
            }

            var sourceType = datas.First().GetType();
            if (typeof(string) == sourceType || typeof(char) == sourceType)
            {
                return string.Join(SqlKeyWorld.Split1, datas.Select(d => string.Concat(SqlKeyWorld.Split2, d, SqlKeyWorld.Split2)));
            }

            return string.Join(SqlKeyWorld.Split1, datas);
        }
    }
}
