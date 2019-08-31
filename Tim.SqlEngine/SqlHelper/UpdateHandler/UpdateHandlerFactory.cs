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
            var types = ReflectUtil.ReflectUtil.GetSubTypes(typeof(BaseUpdateHandler));
            foreach (var type in types)
            {
                BaseUpdateHandler updateHandler = (BaseUpdateHandler)Activator.CreateInstance(type);
                RegisertUpdateHandler(updateHandler.Type, updateHandler);
            }
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
            updateHandlers.Add(type, queryHandler);
        }
    }
}
