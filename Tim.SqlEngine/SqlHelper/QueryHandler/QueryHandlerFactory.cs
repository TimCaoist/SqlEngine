using System;
using System.Collections.Generic;
using Tim.SqlEngine.Models;

namespace Tim.SqlEngine.SqlHelper.QueryHandler
{
    public static class QueryHandlerFactory
    {
        private readonly static Dictionary<int, IQueryHandler> queryHandlers = new Dictionary<int, IQueryHandler>();

        static QueryHandlerFactory()
        {
            var types = ReflectUtil.ReflectUtil.GetSubTypes(typeof(BaseQueryHandler));
            foreach (var type in types)
            {
                BaseQueryHandler queryHandler = (BaseQueryHandler)Activator.CreateInstance(type);
                RegisertQueryHandler(queryHandler.Type, queryHandler);
            }
        }

        public static IQueryHandler GetQueryHandler(int type)
        {
            IQueryHandler queryHandler;
            if (queryHandlers.TryGetValue(type, out queryHandler))
            {
                return queryHandler;
            }

            return null;
        }

        public static void RegisertQueryHandler(int type, IQueryHandler queryHandler)
        {
            queryHandlers.Add(type, queryHandler);
        }
    }
}
