using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tim.SqlEngine.Models;

namespace Tim.SqlEngine.PlugIn
{
    public interface IUpdateValueGetter
    {
        object Get(string dbKey, string tableName, ColumnRule columnRule, object paramData);
    }
}
