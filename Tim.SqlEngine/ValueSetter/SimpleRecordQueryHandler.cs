using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using Tim.SqlEngine.Models;

namespace Tim.SqlEngine.ValueSetter
{
    public class SimpleRecordQueryHandler : IValueSetter
    {
        public object CreateInstance()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<string> GetFields(object data)
        {
            throw new NotImplementedException();
        }

        public object GetValue(object data, string key)
        {
            throw new NotImplementedException();
        }

        public void SetField(string filed, object data)
        {
            throw new NotImplementedException();
        }

        public void SetField(object parent, object obj, string field)
        {
            throw new NotImplementedException();
        }

        public void SetFieldByConfig(object parent, object obj, QueryConfig queryConfig)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<object> SetterDatas(QueryConfig queryConfig, MySqlDataReader dataReader, IEnumerable<string> columns)
        {
            string field = string.Empty;
            if (columns.Count() == 1)
            {
                field = columns.First();
            }
            else {
                field = queryConfig.Config["simple_field"].ToString();
            }

            ICollection<object> datas = new List<object>();
            while (dataReader.Read())
            {
                var val = dataReader[field];
                datas.Add(val);
            }

            return datas;
        }
    }
}
