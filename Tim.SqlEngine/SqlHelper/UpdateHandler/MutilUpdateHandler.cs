using System;
using System.Collections.Generic;
using Tim.SqlEngine.Models;

namespace Tim.SqlEngine.SqlHelper.UpdateHandler
{
    /// <summary>
    /// 主要用来组装更新操作
    /// </summary>
    public class MutilUpdateHandler : BaseBatchUpdateHandler
    {
        public override int Type => 7;

        protected override object DoUpdate(UpdateContext context)
        {
            var handlerConfig = context.HandlerConfig;
            var configs = context.Configs;
            var updateParam = context.Params;
            var complexData = context.ComplexData;
            IDictionary<string, object> dictDatas = SetContentData(context, complexData);
            foreach (var config in configs)
            {
                IUpdateHandler queryHandler = UpdateHandlerFactory.GetUpdateHandler(config.QueryType);
                context.Configs = new UpdateConfig[] { config };
                var data = queryHandler.Update(context);
                if (string.IsNullOrEmpty(config.Filed))
                {
                    continue;
                }

                //主要是争对查询结果储存，可能后面更改会用到
                dictDatas.Add(config.Filed, data);
            }

            return true;
        }

        protected override object DoUpdate(UpdateContext context, UpdateConfig config, IEnumerable<object> datas, object complexData)
        {
            throw new NotImplementedException();
        }
    }
}
