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
        public static IEnumerable<object> GetFilterValues(Dictionary<string, string> matcherFields, object parent, IEnumerable<object> datas)
        {
            if (!matcherFields.Any())
            {
                return datas;
            }

            ICollection<object> matchDatas = new List<object>();
            if (matcherFields.Count() == 1)
            {
                var mf = matcherFields.First();
                var parentValue = ReflectUtil.ReflectUtil.GetProperty(parent, mf.Key);
                foreach (var data in datas)
                {
                    var childValue = ReflectUtil.ReflectUtil.GetProperty(data, mf.Value);
                    if (childValue.Equals(parentValue))
                    {
                        matchDatas.Add(data);
                    }
                }

                return matchDatas;
            }

            foreach (var data in datas)
            {
                var isMatch = true;
                foreach (var mf in matcherFields)
                {
                    var parentValue = ReflectUtil.ReflectUtil.GetProperty(parent, mf.Key);
                    var childValue = ReflectUtil.ReflectUtil.GetProperty(data, mf.Value);
                    if (childValue.Equals(parentValue))
                    {
                        continue;
                    }

                    isMatch = false;
                    break;
                }

                if (!isMatch)
                {
                    continue;
                }

                matchDatas.Add(data);
            }

            return matchDatas;
        }

        public static ValueInfo GetValue(string field, object data)
        {
            var isArray = ReflectUtil.ReflectUtil.IsArray(data);
            if (!isArray)
            {
                var valueInfo = new ValueInfo
                {
                    Data = ReflectUtil.ReflectUtil.GetProperty(data, field)
                };

                valueInfo.IsArray = ReflectUtil.ReflectUtil.IsArray(valueInfo.Data);
                return valueInfo;
            }

            IDictionary<string, object> dictData = data as IDictionary<string, object>;
            if (dictData != null)
            {
                var findData = dictData[field];
                var valueInfo = new ValueInfo
                {
                    Data = findData
                };

                valueInfo.IsArray = ReflectUtil.ReflectUtil.IsArray(findData);
                return valueInfo;
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
            return Builder(valueInfo.Data, valueInfo.IsArray);
        }

        public static string Builder(object data, bool isArray)
        {
            if (isArray == false)
            {
                return data.ToString();
            }

            var datas = data as IEnumerable<object>;
            if (datas == null)
            {
                return TryBuilder(data);
            }

            if (datas.Any() == false)
            {
                return string.Empty;
            }

            var type = datas.GetType();
            var sourceType = type.GetElementType();
            if (sourceType == null)
            {
                sourceType = type.GetGenericArguments()[0];
            }

            if (typeof(string) == sourceType || typeof(char) == sourceType)
            {
                return string.Join(SqlKeyWorld.Split1, datas.Distinct().Select(d => string.Concat(SqlKeyWorld.Split2, d, SqlKeyWorld.Split2)));
            }

            

            return string.Join(SqlKeyWorld.Split1, datas.Distinct());
        }

        public static string TryBuilder(object data)
        {
            var elementType = data.GetType().GetElementType();
            if (elementType == typeof(int))
            {
                var datas = (IEnumerable<int>)data;
                return string.Join(SqlKeyWorld.Split1, datas.Distinct());
            }
            else if (elementType == typeof(long)) {
                var datas = (IEnumerable<long>)data;
                return string.Join(SqlKeyWorld.Split1, datas.Distinct());
            }
            else if (elementType == typeof(string))
            {
                var datas = (IEnumerable<string>)data;
                return string.Join(SqlKeyWorld.Split1, datas.Distinct().Select(d => string.Concat(SqlKeyWorld.Split2, d, SqlKeyWorld.Split2)));
            }
            else if (elementType == typeof(char))
            {
                var datas = (IEnumerable<char>)data;
                return string.Join(SqlKeyWorld.Split1, datas.Distinct().Select(d => string.Concat(SqlKeyWorld.Split2, d, SqlKeyWorld.Split2)));
            }
            else if (elementType == typeof(decimal))
            {
                var datas = (IEnumerable<decimal>)data;
                return string.Join(SqlKeyWorld.Split1, datas.Distinct());
            }
            else if (elementType == typeof(double))
            {
                var datas = (IEnumerable<double>)data;
                return string.Join(SqlKeyWorld.Split1, datas.Distinct());
            }
            else if (elementType == typeof(float))
            {
                var datas = (IEnumerable<float>)data;
                return string.Join(SqlKeyWorld.Split1, datas.Distinct());
            }

            return string.Empty;
        }
    }
}
