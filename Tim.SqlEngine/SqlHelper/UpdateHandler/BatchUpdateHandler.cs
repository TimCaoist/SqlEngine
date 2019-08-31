using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tim.SqlEngine.Common;
using Tim.SqlEngine.Models;
using Tim.SqlEngine.SqlHelper.QueryHandler;
using Tim.SqlEngine.ValueSetter;

namespace Tim.SqlEngine.SqlHelper.UpdateHandler
{
    public class BatchUpdateHandler : BaseBatchUpdateHandler
    {
        public override int Type => 5;

        private const int PerCount = 50;

        private const string End = "END";

        protected override object DoUpdate(UpdateContext context, UpdateConfig config, IEnumerable<object> datas, object complexData)
        {
            var sql = config.Sql;
            var cols = GetCols(config);
            var key = GetKeyName(config, cols);
            IValueSetter valueSetter = ValueSetterCreater.Create(datas.First());

            //1.自定义SQl一条条插入
            if (!string.IsNullOrEmpty(sql) || !string.IsNullOrEmpty(config.Filter))
            {
                if (string.IsNullOrEmpty(sql))
                {
                    config.Sql = DBHelper.BuildUpdateSql(cols, config, key, SqlKeyWorld.ComplexDataObjectStart);
                }

                config.ReturnId = true;
                var keys = valueSetter.GetFields(datas.First());
                foreach (var data in datas)
                {
                    context.ContentParams.ReplaceOrInsert(SqlKeyWorld.ComplexData, data);
                    UpdateTrigger.TriggeValuesChecked(context, data, config, cols, ActionType.Update, valueSetter, keys);
                    SqlExcuter.ExcuteTrann(context);
                    ExcuteSubUpdate(context, config, data);
                }

                return datas.Count();
            }

            return UpdateOnOneTime(context, config, cols, datas, valueSetter, key);
        }

        private object UpdateOnOneTime(UpdateContext context, UpdateConfig config, IDictionary<string, string> cols, IEnumerable<object> datas, IValueSetter valueSetter, string key)
        {
            var columnInfos = TableColumnQueryHandler.QueryColumns(config).Where(c => cols.Keys.Contains(c.ColName) && DBHelper.SpecailColumn(c)).ToArray();

            StringBuilder sb = new StringBuilder();
            var len = datas.Count();
            var keys = valueSetter.GetFields(datas.First());
            var page = len / PerCount;
            if (page % PerCount != 0)
            {
                page++;
            }

            if (page == 0 && len > 0)
            {
                page = 1; 
            }

            var cCount = cols.Count();
            for (var p = 0; p < page; p++)
            {
                var currentIndex = p * PerCount;
                ICollection<object> ids = new List<object>();
                IDictionary<string, StringBuilder> dictSbs = new Dictionary<string, StringBuilder>();
                for (var i = currentIndex; i < currentIndex + PerCount; i++)
                {
                    if (i >= len)
                    {
                        break;
                    }

                    var data = datas.ElementAt(i);
                    context.ContentParams.ReplaceOrInsert(SqlKeyWorld.ComplexData, data);
                    UpdateTrigger.TriggeValuesChecked(context, data, config, cols, ActionType.Update, valueSetter, keys);
                    var id = valueSetter.GetValue(data, key);
                    ids.Add(id);

                    for (var c = 0; c < cCount; c++)
                    {
                        var col = cols.ElementAt(c);
                        if (col.Key.Equals(key, StringComparison.OrdinalIgnoreCase))
                        {
                            continue;
                        }

                        StringBuilder colSb;
                        if (!dictSbs.TryGetValue(col.Key, out colSb))
                        {
                            colSb = new StringBuilder();
                            dictSbs.Add(col.Key, colSb);
                        }

                        colSb.Append($" WHEN '{id}' THEN {DBHelper.BuildColVal(col, valueSetter, data, columnInfos)} ");
                    }
                }

                if (ids.Any() == false)
                {
                    break;
                }

                sb.AppendFormat(DBHelper.UpdateFormatter, config.Table);
                var index = 0;
                foreach (var colSb in dictSbs)
                {
                    sb.Append(string.Concat("`", colSb.Key, "` = CASE `", key, "` "));
                    sb.Append(colSb.Value.ToString());
                    sb.Append(End);
                    index++;

                    if (index < cCount)
                    {
                        sb.Append(SqlKeyWorld.Split3);
                    }
                }

                sb.Append($"{DBHelper.Where}{key} {SqlKeyWorld.In} ({string.Join(SqlKeyWorld.Split1, ids)});");
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
