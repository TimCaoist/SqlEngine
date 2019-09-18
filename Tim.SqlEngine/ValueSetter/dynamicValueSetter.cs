using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using Tim.SqlEngine.Models;
using Tim.SqlEngine.Parser;

namespace Tim.SqlEngine.ValueSetter
{
    public class DynamicValueSetter : IValueSetter
    {
        private IDictionary<string, object> instance;

        public object CreateInstance()
        {
            instance = new ExpandoObject();
            return instance;
        }

        public IEnumerable<object> SetterDatas(QueryConfig queryConfig, MySqlDataReader dataReader, IEnumerable<string> columns)
        {
            ICollection<object> datas = new List<object>();
            var alias = queryConfig.Alais;
            var count = columns.Count();
            var alaisParser = new AlaisParser(columns, alias);
            while (dataReader.Read())
            {
                dynamic data = new ExpandoObject();
                for (var i = 0; i < count; i++)
                {
                    var col = columns.ElementAt(i);
                    ((IDictionary<string, object>)data).Add(alaisParser.GetName(i, col), dataReader[col]);;
                }

                datas.Add(data);
            }

            return datas;
        }

        public void SetField(string filed, object data)
        {
            SetField(instance, data, filed);
        }

        public void SetField(object parent, object obj, string field)
        {
            var instance = ((IDictionary<string, object>)parent);
            object existData;
            if (!instance.TryGetValue(field, out existData))
            {
                instance.Add(field, obj);
                return;
            }

            if (!(existData is List<object> datas))
            {
                throw new ArgumentException("已存在相同的字段!");
            }

            var insertDatas = (IEnumerable<object>)obj;
            foreach (var d in insertDatas)
            {
                if (datas.Contains(d))
                {
                    continue;
                }

                datas.Add(d);
            }
        }

        public IEnumerable<string> GetFields(object data)
        {
            return ((IDictionary<string, object>)data).Keys;
        }

        public object GetValue(object data, string key)
        {
            return ((IDictionary<string, object>)data)[key];
        }
    }
}
