using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tim.SqlEngine.Common;
using Tim.SqlEngine.Models;

namespace Tim.SqlEngine.SqlHelper.UpdateHandler
{
    public class SimpleDeleteHandler : BaseSimpleUpdateHandler
    {
        public override int Type => 3;

        public override string BuilderSql(UpdateContext updateContext, UpdateConfig config, IDictionary<string, string> cols)
        {
            var key = GetKeyName(config, cols);
            return DBHelper.BuildDeleteSql(cols, config, key, SqlKeyWorld.ParamStart);
        }

        protected override void ApplyRules(UpdateContext updateContext, UpdateConfig config, IDictionary<string, string> cols)
        {
        }
    }
}
