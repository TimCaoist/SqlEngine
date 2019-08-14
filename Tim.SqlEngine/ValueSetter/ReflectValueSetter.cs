using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using Tim.SqlEngine.Models;

namespace Tim.SqlEngine.ValueSetter
{
    public class ReflectValueSetter : IValueSetter
    {
        private readonly string AssemblyString;

        private readonly string TypeStr;
        public ReflectValueSetter(string assemblyString, string typeStr)
        {
            AssemblyString = assemblyString;
            TypeStr = typeStr;
        }

        public IEnumerable<object> SetterDatas(QueryConfig queryConfig, MySqlDataReader dataReader, IEnumerable<string> columns)
        {
            ICollection<object> datas = new List<object>();
            while (dataReader.Read())
            {
                object data = ReflectUtil.ReflectUtil.CreateInstance(AssemblyString, TypeStr);
                var type = data.GetType();
                foreach (var col in columns)
                {
                    var property = type.GetProperty(col);
                    if (property == null)
                    {
                        continue;
                    }

                    property.SetValue(data, dataReader[col]);
                }

                datas.Add(data);

                if (queryConfig.OnlyOne) {
                    break;
                }
            }

            return datas;
        }
    }
}
