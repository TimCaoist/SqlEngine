using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tim.SqlEngine.Models;

namespace Tim.SqlEngine.Common
{
    public static class DBHelper
    {
        private const char Split = '\'';

        public static string GetDBValue(object data, IEnumerable<Column> columns)
        {
            if (columns.Any() == false)
            {
                return string.Concat(Split, data.ToString(), Split);
            }

            return string.Concat(Split, data.ToString(), Split);
        }

        public static bool SpecailColumn(Column column)
        {
            return false;
        }
    }
}
