using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tim.SqlEngine.ValueSetter.ValueChecked
{
    public class MinMaxValueChecked : LengthValueChecked
    {
        public new readonly static LengthValueChecked Instance = new LengthValueChecked();

        protected override bool DoCheckMax(decimal max, object val)
        {
            return decimal.Parse(val.ToString()) <= max;
        }

        protected override bool DoCheckMin(decimal min, object val)
        {
            return decimal.Parse(val.ToString()) >= min;
        }
    }
}
