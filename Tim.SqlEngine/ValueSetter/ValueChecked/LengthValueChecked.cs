using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tim.SqlEngine.Models;

namespace Tim.SqlEngine.ValueSetter.ValueChecked
{
    public class LengthValueChecked : IValueChecked
    {
        public readonly static LengthValueChecked Instance = new LengthValueChecked();

        public bool Checked(UpdateContext updateContext, ColumnRule mc, object data, string key, string realKey)
        {
            var lenStrs = mc.Value.ToString().Split(SqlKeyWorld.Split3);
            bool result;
            if (lenStrs.Length == 1)
            {
                result = DoCheckMax(decimal.Parse(lenStrs[0]), data);
                return result;
            }

            result = DoCheckMax(decimal.Parse(lenStrs[1]), data);
            if (result == false)
            {
                return false;
            }

            result = DoCheckMin(decimal.Parse(lenStrs[0]), data);
            return result;
        }

        protected virtual bool DoCheckMax(decimal max, object val)
        {
            return val.ToString().Length <= max;
        }

        protected virtual bool DoCheckMin(decimal min, object val)
        {
            return val.ToString().Length >= min;
        }
    }
}
