using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tim.SqlEngine.Models;
using Tim.SqlEngine.ValueSetter;
using Tim.SqlEngine.Common;
using Tim.SqlEngine.SqlHelper.QueryHandler;
using Newtonsoft.Json;

namespace Tim.SqlEngine.SqlHelper.UpdateHandler
{
    public class BatchInsertHandler : BaseUpdateHandler
    {
        public override int Type => 4;

        private const string BatchFieldPath = "batch_field";

        /// <summary>
        /// 忽略设置Key
        /// </summary>
        private const string IngoreKey = "ingore_key";

        private const string RelatedInserts = "sub_updates";

        public override object Update(UpdateContext context)
        {
            var config = context.Config;
            if (string.IsNullOrEmpty(config.Connection))
            {
                config.Connection = context.HandlerConfig.Connection;
            }

            var field = config.Config[BatchFieldPath].ToString();
            var inputData = ValueSetter.ValueGetter.GetValue(field, context.ComplexData);
            IEnumerable<object> datas;
            if (inputData.IsArray)
            {
                datas = (IEnumerable<object>)inputData.Data;
                if (datas.Any() == false)
                {
                    return 0;
                }
            }
            else {
                datas = new object[] { inputData.Data };
            }
            
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
                    var colStrs = cols.Values.Select(v => {
                        if (v.StartsWith(SqlKeyWorld.ParamStart))
                        {
                            return v;
                        }

                        return string.Concat("@v_cd.", v, " ");
                    });

                    config.Sql = $"insert into {config.Table} ({string.Join(SqlKeyWorld.Split1, cols.Keys)}) values ({string.Join(SqlKeyWorld.Split1, colStrs)})";
                }
                
                config.ReturnId = true;
                var keys = valueSetter.GetFields(datas.First());
                foreach (var data in datas)
                {
                    context.ContentParams.Clear();
                    context.ContentParams.ReplaceOrInsert(SqlKeyWorld.ComplexData, data);
                    UpdateTrigger.TriggeDefaultValues(context, data, config, cols, valueSetter, keys);
                    UpdateTrigger.TriggeValuesChecked(context, data, config, cols, ActionType.Insert, valueSetter, keys);
                    var id = (long)SqlExcuter.ExcuteTrann(context);
                    valueSetter.SetField(data, id, key);
                    ExcuteSubInsert(context, config, data);
                }

                return datas.Count();
            }

            return InsertOnOneTimes(context, config, cols, datas, valueSetter);
        }

        private object InsertOnOneTimes(UpdateContext context, UpdateConfig config, IDictionary<string, string> cols, IEnumerable<object> datas, IValueSetter valueSetter)
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
                var colVals = cols.Select(c => {
                    var v = c.Value;
                    if (v.StartsWith(SqlKeyWorld.ParamStart))
                    {
                        return v;
                    }

                    var val = valueSetter.GetValue(data, v);
                    return DBHelper.GetDBValue(val, columnInfos);
                });

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
                ExcuteSubInsert(context, config, data);
            }

            return result;
        }

        public void ExcuteSubInsert(UpdateContext context, UpdateConfig updateConfig, object parent)
        {
            var config = updateConfig.Config;
            if (config == null)
            {
                return;
            }

            var subConfigs = config[RelatedInserts];
            if (subConfigs == null)
            {
                return;
            }

            var relatedConfigs = subConfigs.ToObject<IEnumerable<UpdateConfig>>();
            if (relatedConfigs.Any() == false)
            {
                return;
            }

            foreach (var relatedConfig in relatedConfigs)
            {
                relatedConfig.InTran = true;
                IUpdateHandler queryHandler = UpdateHandlerFactory.GetUpdateHandler(relatedConfig.QueryType);
                var subContext = new UpdateContext(context)
                {
                    ComplexData = parent,
                    Data = parent,
                    Configs = new UpdateConfig[] { relatedConfig }
                };

                queryHandler.Update(subContext);
            }
        }
    }
}
