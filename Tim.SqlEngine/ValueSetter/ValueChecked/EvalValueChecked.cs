using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tim.LambdaEngine;
using Tim.SqlEngine.Models;
using Tim.SqlEngine.Parser;

namespace Tim.SqlEngine.ValueSetter.ValueChecked
{
    public class EvalValueChecked : IValueChecked
    {
        public readonly static EvalValueChecked Instance = new EvalValueChecked();

        public bool Checked(UpdateContext updateContext, ColumnRule mc, KeyValuePair<string, object> param, string realKey)
        {
            var eval = mc.Value.ToString();
            var usedParams = SqlParser.GetApplyParamRuleSql(updateContext, eval).Item2;
            var right = (bool)LambdaEnginer.Eval(eval, usedParams);
            return right;
        }
    }
}
