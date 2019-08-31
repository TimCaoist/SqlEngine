using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tim.SqlEngine.Common;
using Tim.SqlEngine.Models;
using Tim.SqlEngine.ValueSetter;

namespace Tim.SqlEngine.SqlHelper.UpdateHandler
{
    public class SimpleUpdateHandler : BaseSimpleUpdateHandler
    {
        public override int Type => 2;

        public override string BuilderSql(UpdateContext updateContext, UpdateConfig config, IDictionary<string, string> cols)
        {
            var key = GetKeyName(config, cols);
            var sql = DBHelper.BuildUpdateSql(cols, config, key, SqlKeyWorld.ParamStart);
            return sql.ToString();
        }

        protected override void ApplyRules(UpdateContext updateContext, UpdateConfig config, IDictionary<string, string> cols)
        {
            var uParams = updateContext.Params;
            IValueSetter valueSetter = ValueSetterCreater.Create(uParams);
            UpdateTrigger.TriggeValuesChecked(updateContext, uParams, config, cols, ActionType.Update, valueSetter, uParams.Keys);
        }
    }
}
