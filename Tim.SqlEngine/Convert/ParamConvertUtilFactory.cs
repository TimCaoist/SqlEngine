using System;
using System.Collections.Generic;
using Tim.SqlEngine.Models;

namespace Tim.SqlEngine.Convert
{
    public static class ParamConvertUtilFactory
    {
        private readonly static Dictionary<int, IParamConvert> handlers = new Dictionary<int, IParamConvert>();

        static ParamConvertUtilFactory()
        {
            var types = ReflectUtil.ReflectUtil.GetSubTypes(typeof(IParamConvert));
            foreach (var type in types)
            {
                IParamConvert handler = (IParamConvert)Activator.CreateInstance(type);
                handlers.Add((int)handler.ConvertType, handler);
            }
        }
        public static void RegisertHandler(int type, IParamConvert queryHandler)
        {
            handlers.Add(type, queryHandler);
        }

        public static IParamConvert Create(ParamConvertConfig config) {
            IParamConvert handler;
            if (handlers.TryGetValue(config.ConvertType, out handler))
            {
                return handler;
            }

            return null;
        }
    }
}
