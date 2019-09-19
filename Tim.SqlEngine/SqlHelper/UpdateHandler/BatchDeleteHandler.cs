using System.Collections.Generic;
using System.Linq;
using Tim.SqlEngine.Common;
using Tim.SqlEngine.Models;
using Tim.SqlEngine.ValueSetter;

namespace Tim.SqlEngine.SqlHelper.UpdateHandler
{
    /// <summary>
    /// </summary>
    public class BatchDeleteHandler : BaseBatchUpdateHandler
    {

        /*{
         * Example
  "configs": [
    {
      "table": "organizationroleusers",
      "filter": " UserId = @v_cd.UserId and RoleId = @roleId ",
      "config": {
      }
    }
  ],
  "connection": "sd_organizations",
  "json_type": "SD.PalmClass.Model,SD.PalmClass.Model.Users.User[]",
  "name": "",
  "query_type": 6
}*/
        public override int Type => 6;

        protected override object DoUpdate(UpdateContext context, UpdateConfig config, IEnumerable<object> datas, object complexData)
        {
            var sql = config.Sql;
            var cols = GetCols(config);
            var key = GetKeyName(config, cols);
            IValueSetter valueSetter = ValueSetterCreater.Create(datas.First());

            if (!string.IsNullOrEmpty(sql) || !string.IsNullOrEmpty(config.Filter))
            {
                if (string.IsNullOrEmpty(sql))
                {
                    config.Sql = DBHelper.BuildDeleteSql(cols, config, key, SqlKeyWorld.ComplexDataObjectStart);
                }

                var keys = valueSetter.GetFields(datas.First());
                foreach (var data in datas)
                {
                    context.ContentParams.ReplaceOrInsert(SqlKeyWorld.ComplexData, data);
                    SqlExcuter.ExcuteTrann(context);
                    ExcuteSubUpdate(context, config, data);
                }

                return datas.Count();
            }

            return DeleteOnOneTime(context, config, cols, datas, valueSetter, key);
        }

        private object DeleteOnOneTime(UpdateContext context, UpdateConfig config, IDictionary<string, string> cols, IEnumerable<object> datas, IValueSetter valueSetter, string key)
        {
            ICollection<object> ids = new List<object>();
            foreach (var data in datas)
            {
                var id = valueSetter.GetValue(data, key);
                ids.Add(id);
                ExcuteSubUpdate(context, config, data);
            }

            var sql = $"DELETE FROM {config.Table} where {key} in ({string.Join(SqlKeyWorld.Split1, ids)});";
            config.Sql = sql;
            object result = SqlExcuter.ExcuteTrann(context);
            return result;
        }
    }
}
