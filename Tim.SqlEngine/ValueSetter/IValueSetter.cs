using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using Tim.SqlEngine.Models;

namespace Tim.SqlEngine.ValueSetter
{
    public interface IValueSetter
    {
        object CreateInstance();

        IEnumerable<object> SetterDatas(BaseHadlerConfig config, MySqlDataReader dataReader, IEnumerable<string> columns);

        void SetField(string filed, object data);

        void SetField(object parent, object obj, string field);

        void SetFieldByConfig(object parent, object obj, QueryConfig queryConfig);

        IEnumerable<string> GetFields(object data);

        object GetValue(object data, string key);
    }
}
