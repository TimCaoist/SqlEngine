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
    }
}
