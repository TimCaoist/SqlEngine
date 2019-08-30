using System.Collections.Generic;
using System.Linq;
using Tim.SqlEngine.Models;
using Tim.SqlEngine.ValueSetter;

namespace Tim.SqlEngine.SqlHelper.UpdateHandler
{
    public class SimpleInsertHandler : BaseSimpleUpdateHandler
    {
        public override int Type => 1;

        public override string BuilderSql(UpdateContext updateContext, UpdateConfig config, IDictionary<string, string> cols)
        {
            return $"insert into {config.Table} ({string.Join(SqlKeyWorld.Split1, cols.Keys)}) values ({string.Join(SqlKeyWorld.Split1, cols.Values.Select(v => string.Concat("@", v, " ")))})";
        }

        protected override void ApplyRules(UpdateContext updateContext, UpdateConfig config, IDictionary<string, string> cols)
        {
            var cParams = updateContext.Params;
            IValueSetter valueSetter = ValueSetterCreater.Create(cParams);
            UpdateTrigger.TriggeDefaultValues(updateContext, cParams, config, cols, valueSetter);
            UpdateTrigger.TriggeValuesChecked(updateContext, cParams, config, cols, ActionType.Insert, valueSetter);
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
