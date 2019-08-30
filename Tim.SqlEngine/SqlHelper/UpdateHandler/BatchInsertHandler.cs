using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tim.SqlEngine.Models;
using Tim.SqlEngine.ValueSetter;
using Tim.SqlEngine.Common;

namespace Tim.SqlEngine.SqlHelper.UpdateHandler
{
    public class BatchInsertHandler : BaseUpdateHandler
    {
        public override int Type => 4;

        private const string BatchFieldPath = "batch_field";

        private const string IngoreKey = "ingore_key";
        public override object Update(UpdateContext context)
        {
            var config = context.Config;
            if (string.IsNullOrEmpty(config.Connection))
            {
                config.Connection = context.HandlerConfig.Connection;
            }

            var field = config.Config[BatchFieldPath].ToString();
            var datas = (IEnumerable<object>)ValueSetter.ValueGetter.GetValue(field, context.ComplexData).Data;
            if (datas.Any() == false)
            {
                return 0;
            }

            var sql = config.Sql;
            var ingoreKey = config.Config[IngoreKey].ToSingleData<bool>();
            var cols = GetCols(config);
            var key = GetKeyName(config, cols);
            config.ReturnId = true;
            IValueSetter valueSetter = ValueSetterCreater.Create(datas.First());
            if (!string.IsNullOrEmpty(sql))
            {
                foreach (var data in datas)
                {
                    context.ContentParams.ReplaceOrInsert(SqlKeyWorld.ComplexData, data);
                    UpdateTrigger.TriggeDefaultValues(context, data, config, cols, valueSetter);
                    UpdateTrigger.TriggeValuesChecked(context, data, config, cols, ActionType.Insert, valueSetter);
                    var id = (long)SqlExcuter.ExcuteTrann(context);

                    if (ingoreKey == true)
                    {
                        continue;
                    }

                    valueSetter.SetField(key, data);
                }

                return datas.Count();
            }

            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"insert into {config.Table} ({string.Join(SqlKeyWorld.Split1, cols.Keys)}) values ");
            foreach (var data in datas)
            {
                context.ContentParams.ReplaceOrInsert(SqlKeyWorld.ComplexData, data);
                UpdateTrigger.TriggeDefaultValues(context, data, config, cols, valueSetter);
                UpdateTrigger.TriggeValuesChecked(context, data, config, cols, ActionType.Insert, valueSetter);
                sb.AppendLine("()");
            }

            return null;
        }
    }
}
