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
            var simpleHandler = new SimpleQueryHandler();
            RegisertQueryHandler(simpleHandler.Type, simpleHandler);

            var mutilQueryHandler = new MutilQueryHandler();
            RegisertQueryHandler(mutilQueryHandler.Type, mutilQueryHandler);

            var singleFieldQueryHandler = new SingleFieldQueryHandler();
            RegisertQueryHandler(singleFieldQueryHandler.Type, singleFieldQueryHandler);
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
            if (queryHandlers.ContainsKey(type))
            {
                return;
            }

            queryHandlers.Add(type, queryHandler);
        }
    }
}
