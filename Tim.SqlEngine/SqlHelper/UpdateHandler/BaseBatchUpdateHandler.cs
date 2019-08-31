using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tim.SqlEngine.Common;
using Tim.SqlEngine.Models;
using Tim.SqlEngine.ValueSetter;

namespace Tim.SqlEngine.SqlHelper.UpdateHandler
{
    public abstract class BaseBatchUpdateHandler : BaseUpdateHandler
    {
        protected const string BatchFieldPath = "batch_field";

        private const string RelatedUpdates = "sub_updates";

        protected static IEnumerable<object> GetDatas(UpdateContext context, UpdateConfig config, object complexData)
        {
            var field = config.Config[BatchFieldPath].ToSingleData<string>(string.Empty);

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

            if (!isArray)
            {
                return new object[] { inputData };
            }

            return (IEnumerable<object>)inputData;
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
