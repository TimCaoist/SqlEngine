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
            config.ReturnId = true;
            UpdateTrigger.TriggeDefaultValues(updateContext, config, cols);
            UpdateTrigger.TriggeValuesChecked(updateContext, config, cols, ActionType.Insert);
            return $"insert into {config.Table} ({string.Join(SqlKeyWorld.Split1, cols.Keys)}) values ({string.Join(SqlKeyWorld.Split1, cols.Values.Select(v => string.Concat("@", v, " ")))})";
        }

        public override object Update(UpdateContext context)
        {
            var result = base.Update(context);
            if (result.ToString() != "0")
            {
                return result;
            }

            object val;
            if (context.Params.TryGetValue(Id, out val))
            {
                return val;
            }

            return result;
        }
    }
}
