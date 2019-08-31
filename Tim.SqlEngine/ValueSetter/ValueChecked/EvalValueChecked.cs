using System;
using Tim.SqlEngine.Common;
using Tim.SqlEngine.Models;

namespace Tim.SqlEngine.ValueSetter.ValueChecked
{
    public class EvalValueChecked : IValueChecked
    {
        public readonly static EvalValueChecked Instance = new EvalValueChecked();

        public bool Checked(UpdateContext context, ColumnRule mc, object data, string key, string realKey)
        {
            var eval = mc.Value.ToString();
            var action = context.ContentParams.CreateOrGet<string, object, Delegate>(eval, expression =>
            {
                return EvalHelper.GetDelegate(context, expression, data);
            });

            return (bool)action.DynamicInvoke(data);
        }
    }
}
