using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tim.SqlEngine.Models;

namespace Tim.SqlEngine.ValueSetter.ValueChecked
{
    public class IsInValueChecked : IValueChecked
    {
        public readonly static IsInValueChecked Instance = new IsInValueChecked();

        public bool Checked(UpdateContext updateContext, ColumnRule mc, KeyValuePair<string, object> param, string realKey)
        {
            IEnumerable<object> array = (IEnumerable<object>)mc.Value;
            return array.Any(a => a.ToString() == param.Value.ToString());
        }
    }
}
