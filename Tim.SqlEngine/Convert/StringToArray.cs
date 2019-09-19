using System;
using System.Linq;
using Tim.SqlEngine.Models;

namespace Tim.SqlEngine.Convert
{
    public class StringToArray : IParamConvert
    {
        public ParamConvertType ConvertType {
            get {
                return ParamConvertType.StringToArray;
            }
        }

        public void DoConvert(ParamConvertConfig c, IContext context)
        {
            object data;
            if (context.Params.TryGetValue(c.Name, out data) == false)
            {
                return;
            }

            Type desType;
            if (c.Args == null || c.Args.Length == 0)
            {
                desType = typeof(long);
            }
            else {
                desType = Type.GetType(c.Args.First());
            }

            var val = data.ToString().Split(SqlKeyWorld.Split3);
            var newVal = val.Select(v => System.Convert.ChangeType(v, desType)).ToArray();
            ParamConvertUtil.StoreToParams(context, c, newVal);
        }
    }
}
