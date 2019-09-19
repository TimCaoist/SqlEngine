using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tim.SqlEngine.Models;

namespace Tim.SqlEngine.Convert
{
    public static class ParamConvertUtil
    {
        public static void DoConvert(IContext context)
        {
            var paramConfigs = context.GetHandlerConfig().ParamConfigs;
            if (paramConfigs == null || !paramConfigs.Any())
            {
                return;
            }

            foreach (var c in paramConfigs)
            {
                IParamConvert paramConvert = ParamConvertUtilFactory.Create(c);
                if (paramConvert == null)
                {
                    continue;
                }

                paramConvert.DoConvert(c, context);
            }
        }

        public static void StoreToParams(IContext context, ParamConvertConfig c, object newVal)
        {
            var storeName = !string.IsNullOrEmpty(c.NewName) ? c.NewName : c.Name;
            if (!context.Params.ContainsKey(storeName))
            {
                context.Params.Add(storeName, newVal);
            }
            else
            {
                context.Params[storeName] = newVal;
            }
        }
    }
}
