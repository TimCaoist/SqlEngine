using System.Collections.Generic;
using System.Linq;
using Tim.SqlEngine.Models;

namespace Tim.SqlEngine.SqlHelper.UpdateHandler
{
    public class SimpleInsertHandler : BaseSimpleUpdateHandler
    {
        public override int Type => 1;

        public override string BuilderSql(UpdateConfig config, IDictionary<string, string> cols)
        {
            config.ReturnId = true;
            return $"insert into {config.Table} ({string.Join(SqlKeyWorld.Split1, cols.Keys)}) values ({string.Join(SqlKeyWorld.Split1, cols.Values.Select(v => string.Concat("@", v, " ")))})";
        }
    }
}
