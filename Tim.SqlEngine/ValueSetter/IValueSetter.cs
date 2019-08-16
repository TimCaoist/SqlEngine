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

        IEnumerable<object> SetterDatas(QueryConfig queryConfig, MySqlDataReader dataReader, IEnumerable<string> columns);
        void SetterField(string filed, object data);
    }
}
