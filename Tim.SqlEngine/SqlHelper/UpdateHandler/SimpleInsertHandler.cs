using System.Collections.Generic;
using System.Linq;
using Tim.SqlEngine.Common;
using Tim.SqlEngine.Models;
using Tim.SqlEngine.ValueSetter;

namespace Tim.SqlEngine.SqlHelper.UpdateHandler
{
    public class SimpleInsertHandler : BaseSimpleUpdateHandler
    {
        public override int Type => 1;

        public override string BuilderSql(UpdateContext updateContext, UpdateConfig config, IDictionary<string, string> cols)
        {
            return DBHelper.BuildInsertSql(cols, config.Table, SqlKeyWorld.ParamStart);
        }

        protected override void ApplyRules(UpdateContext updateContext, UpdateConfig config, IDictionary<string, string> cols)
        {
            var cParams = updateContext.Params;
            IValueSetter valueSetter = ValueSetterCreater.Create(cParams);
            var keys = cParams.Keys;
            UpdateTrigger.TriggeDefaultValues(updateContext, cParams, config, cols, valueSetter, keys);
            UpdateTrigger.TriggeValuesChecked(updateContext, cParams, config, cols, ActionType.Insert, valueSetter, keys);
            config.ReturnId = true;

            var key = GetKeyName(config, cols);
            object id;
            if (updateContext.Params.TryGetValue(key, out id) == false)
            {
                return;
            }

            updateContext.ContentParams.Add(SqlKeyWorld.ReturnKey, id);
        }
    }
}
