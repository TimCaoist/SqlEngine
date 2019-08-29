using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tim.SqlEngine.Models;

namespace Tim.SqlEngine.ValueSetter.ValueChecked
{
    public static class ValueCheckedFactory
    {
        public static IValueChecked Create(ColumnValueType valueType)
        {
            switch (valueType)
            {
                case ColumnValueType.Eval:
                    return EvalValueChecked.Instance;
                case ColumnValueType.IsIn:
                    return IsInValueChecked.Instance;
                case ColumnValueType.Equlas:
                    return EqualValueChecked.Instance;
            }

            return null;
        }
    }
}
