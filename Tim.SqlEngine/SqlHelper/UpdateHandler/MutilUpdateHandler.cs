using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tim.SqlEngine.Models;

namespace Tim.SqlEngine.SqlHelper.UpdateHandler
{
    public class MutilUpdateHandler : BaseBatchUpdateHandler
    {
        public override int Type => 7;

        public override object Update(UpdateContext context)
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
            }

            return true;
        }
    }
}
