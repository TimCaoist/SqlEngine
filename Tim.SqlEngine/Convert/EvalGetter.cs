using System.Linq;
using Tim.SqlEngine.Common;
using Tim.SqlEngine.Models;

namespace Tim.SqlEngine.Convert
{
    public class EvalGetter : IParamConvert
    {
        public ParamConvertType ConvertType => ParamConvertType.EvalGetter;

        public void DoConvert(ParamConvertConfig c, IContext context)
        {
            var arg = c.Args.First();
            var delegte1 = EvalHelper.GetDelegate(context, arg);
            ParamConvertUtil.StoreToParams(context, c, delegte1.DynamicInvoke());
        }
    }
}
