using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tim.SqlEngine.Common;
using Tim.SqlEngine.Models;
using Tim.SqlEngine.SqlHelper.QueryHandler;
using Tim.SqlEngine.ValueSetter;

namespace Tim.SqlEngine.SqlHelper.UpdateHandler
{
    public class BatchInsertHandler : BaseBatchUpdateHandler
    {
        public override int Type => 4;

        /// <summary>
        /// 忽略设置Key
        /// </summary>
        private const string IngoreKey = "ingore_key";

        protected override object DoUpdate(UpdateContext context, UpdateConfig config, IEnumerable<object> datas, object complexData)
        {
            var sql = config.Sql;
            var ingoreKey = config.Config[IngoreKey].ToSingleData<bool>();
            var cols = GetCols(config);
            var key = GetKeyName(config, cols);
            IValueSetter valueSetter = ValueSetterCreater.Create(datas.First());

            //1.自定义SQl一条条插入
            //2.未包含Key又没有忽略产生Key说明是要数据库自动生成
            if (!string.IsNullOrEmpty(sql) || (!cols.ContainsKey(key) && !ingoreKey))
            {
                if (string.IsNullOrEmpty(sql))
                {
                    config.Sql = DBHelper.BuildInsertSql(cols, config.Table, SqlKeyWorld.ComplexDataObjectStart);
                }
                
                config.ReturnId = true;
                var keys = valueSetter.GetFields(datas.First());
                foreach (var data in datas)
                {
                    context.ContentParams.ReplaceOrInsert(SqlKeyWorld.ComplexData, data);
                    UpdateTrigger.TriggeDefaultValues(context, data, config, cols, valueSetter, keys);
                    UpdateTrigger.TriggeValuesChecked(context, data, config, cols, ActionType.Insert, valueSetter, keys);
                    var id = (long)SqlExcuter.ExcuteTrann(context);
                    valueSetter.SetField(data, id, key);
                    ExcuteSubUpdate(context, config, data);
                }

                return datas.Count();
            }

            return InsertOnOneTime(context, config, cols, datas, valueSetter);
        }

        private object InsertOnOneTime(UpdateContext context, UpdateConfig config, IDictionary<string, string> cols, IEnumerable<object> datas, IValueSetter valueSetter)
        {
            StringBuilder sb = new StringBuilder();
            var columnInfos = TableColumnQueryHandler.QueryColumns(config).Where(c => cols.Keys.Contains(c.ColName) && DBHelper.SpecailColumn(c)).ToArray();
            sb.AppendLine($"insert into {config.Table} ({string.Join(SqlKeyWorld.Split1, cols.Keys)}) values ");
            var len = datas.Count();
            var keys = valueSetter.GetFields(datas.First());
            for (var i = 0; i < len; i++)
            {
                var data = datas.ElementAt(i);
                context.ContentParams.ReplaceOrInsert(SqlKeyWorld.ComplexData, data);
                UpdateTrigger.TriggeDefaultValues(context, data, config, cols, valueSetter, keys);
                UpdateTrigger.TriggeValuesChecked(context, data, config, cols, ActionType.Insert, valueSetter, keys);

                sb.AppendLine(string.Intern("("));
                var colVals = cols.Select(c => DBHelper.BuildColVal(c, valueSetter, data, columnInfos));
                sb.Append(string.Join(SqlKeyWorld.Split1, colVals));
                sb.Append(string.Intern(")"));
                if (i != len - 1)
                {
                    sb.Append(SqlKeyWorld.Split1);
                }
            }

            config.Sql = sb.ToString();
            object result = SqlExcuter.ExcuteTrann(context);
            foreach (var data in datas)
            {
                ExcuteSubUpdate(context, config, data);
            }

            return result;
        }
    }
}
