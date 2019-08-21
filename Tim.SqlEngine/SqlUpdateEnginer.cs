﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tim.SqlEngine.Models;
using Tim.SqlEngine.SqlHelper;
using Tim.SqlEngine.SqlHelper.UpdateHandler;

namespace Tim.SqlEngine
{
    public static partial class SqlEnginer
    {
        public static object Update(UpdateHandlerConfig handlerConfig, IDictionary<string, object> queryParams = null)
        {
            IUpdateHandler updateHandler = UpdateHandlerFactory.GetUpdateHandler(handlerConfig.QueryType);
            var context = new UpdateContext
            {
                HandlerConfig = handlerConfig,
                Configs = handlerConfig.Configs,
                Params = queryParams
            };

            var result = updateHandler.Update(context);
            if (context.Conns != null && context.Conns.Any())
            {
                context.Submit();
            }

            return result;
        }
    }
}