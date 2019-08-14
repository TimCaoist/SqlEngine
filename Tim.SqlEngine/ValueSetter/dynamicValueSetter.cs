using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using Tim.SqlEngine.Models;

namespace Tim.SqlEngine.ValueSetter
{
    public class DynamicValueSetter : IValueSetter
    {
        public IEnumerable<object> SetterDatas(QueryConfig queryConfig, MySqlDataReader dataReader, IEnumerable<string> columns)
        {
            ICollection<object> datas = new List<object>();
            
            while (dataReader.Read())
            {
                dynamic data = new ExpandoObject();
                foreach (var col in columns)
                {
                    ((IDictionary<string, object>)data).Add(col, dataReader[col]);
                }

                datas.Add(data);
            }

            return datas;
        }
    }
}
