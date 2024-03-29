﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tim.SqlEngine.Models;

namespace Tim.SqlEngine.ValueSetter.ValueChecked
{
    public class EqualValueChecked : IValueChecked
    {
        public readonly static EqualValueChecked Instance = new EqualValueChecked();

        public bool Checked(UpdateContext updateContext, ColumnRule mc, object data, string key, string realKey)
        {
            return mc.Value.ToString() == data.ToString();
        }
    }
}
