using System;
using System.Collections.Generic;
using System.Linq;
using Tim.SqlEngine.Common;
using Tim.SqlEngine.Models;
using Tim.SqlEngine.ValueSetter;

namespace Tim.SqlEngine.SqlHelper.UpdateHandler
{
    public abstract class BaseBatchUpdateHandler : BaseUpdateHandler
    {
        protected const string BatchFieldPath = "batch_field";

        private const string RelatedUpdates = "sub_updates";

        private const string FilterEval = "filter_eval";

        protected override object DoUpdate(UpdateContext context)
        {
            var config = context.Config;
            if (string.IsNullOrEmpty(config.Connection))
            {
                config.Connection = context.HandlerConfig.Connection;
            }

            var complexData = context.ComplexData;
            IEnumerable<object> datas = GetDatas(context, config, complexData);
            if (datas.Any() == false)
            {
                return 0;
            }

            SetContentData(context, complexData);
            return DoUpdate(context, config, datas, complexData);
        }

        protected abstract object DoUpdate(UpdateContext context, UpdateConfig config, IEnumerable<object> datas, object complexData);

        protected static IEnumerable<object> GetDatas(UpdateContext context, UpdateConfig config, object complexData)
        {
            string field = string.Empty;
            if (config.Config != null && config.Config[BatchFieldPath] != null)
            {
               field = config.Config[BatchFieldPath].ToSingleData<string>(string.Empty);
            }

            bool isArray;
            object inputData;
            if (string.IsNullOrEmpty(field))
            {
                isArray = ReflectUtil.ReflectUtil.IsArray(complexData);
                inputData = complexData;
            }
            else
            {
                var array = field.Split('.');
                var valueInfo = ValueGetter.GetValue(array, complexData);
                isArray = valueInfo.IsArray;
                inputData = valueInfo.Data;
            }

            IEnumerable<object> results;
            if (!isArray)
            {
                results = new object[] { inputData };
            }

            results = (IEnumerable<object>)inputData;

            var filterEval = config.Config[FilterEval].ToSingleData<string>(string.Empty);
            if (string.IsNullOrEmpty(filterEval) || results.Any() == false)
            {
                return results;
            }

            var action = context.ContentParams.CreateOrGet<string, object, Delegate>(filterEval, eval =>
            {
                return EvalHelper.GetDelegate(context, filterEval, results.First());
            });
            
            results = results.Where(r => (bool)action.DynamicInvoke(r)).ToArray();
            return results;
        }

        protected static IDictionary<string, object> SetContentData(UpdateContext context, object complexData)
        {
            IDictionary<string, object> dictData;
            if (context.Data == null)
            {
                dictData = new Dictionary<string, object>();
                context.Data = dictData;
            }
            else
            {
                dictData = (IDictionary<string, object>)context.Data;
            }

            if (dictData.ContainsKey(SqlKeyWorld.ShortComplexData))
            {
                return dictData;
            }

            dictData.Add(SqlKeyWorld.ShortComplexData, complexData);
            return dictData;
        }

        protected static void ExcuteSubUpdate(UpdateContext context, UpdateConfig updateConfig, object parent)
        {
            var config = updateConfig.Config;
            if (config == null)
            {
                return;
            }

            var subConfigs = config[RelatedUpdates];
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
                    Configs = new UpdateConfig[] { relatedConfig }
                };

                queryHandler.Update(subContext);
            }
        }
    }
}
