using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tim.SqlEngine.SqlHelper.UpdateHandler
{
    public static class UpdateHandlerFactory 
    {
        private readonly static Dictionary<int, IUpdateHandler> updateHandlers = new Dictionary<int, IUpdateHandler>();

        static UpdateHandlerFactory()
        {
            var simpleHandler = new SimpleInsertHandler();
            RegisertUpdateHandler(simpleHandler.Type, simpleHandler);

            var updateHandler = new SimpleUpdateHandler();
            RegisertUpdateHandler(updateHandler.Type, updateHandler);

            var deleteHandler = new SimpleDeleteHandler();
            RegisertUpdateHandler(deleteHandler.Type, deleteHandler);

            var batchInsertHandler = new BatchInsertHandler();
            RegisertUpdateHandler(batchInsertHandler.Type, batchInsertHandler);
        }

        internal static IUpdateHandler GetUpdateHandler(int type)
        {
            IUpdateHandler updateHandler;
            if (updateHandlers.TryGetValue(type, out updateHandler))
            {
                return updateHandler;
            }

            return null;
        }

        public static void RegisertUpdateHandler(int type, IUpdateHandler queryHandler)
        {
            if (updateHandlers.ContainsKey(type))
            {
                return;
            }

            updateHandlers.Add(type, queryHandler);
        }
    }
}
