using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tim.SqlEngine.Models;

namespace Tim.SqlEngine.ValueSetter.ValueChecked
{
    public interface IValueChecked
    {
        bool Checked(UpdateContext updateContext, ColumnRule mc, object data, string key, string realKey);
    }
}
