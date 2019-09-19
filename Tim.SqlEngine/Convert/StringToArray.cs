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

        /// <summary>
        /// Args 0: 类型
        /// Args 1: 分割字符
        /// </summary>
        public void DoConvert(ParamConvertConfig c, IContext context)
        {
            object data;
            if (context.Params.TryGetValue(c.Name, out data) == false)
            {
                return;
            }

            Type desType;
            char splitChar = SqlKeyWorld.Split3;
            if (c.Args == null || c.Args.Length == 0)
            {
                desType = typeof(long);
            }
            else {
                desType = Type.GetType(c.Args.First());
                if (c.Args.Length > 1)
                {
                    splitChar = c.Args[1][0];
                }
            }

            var val = data.ToString().Split(splitChar);
            var newVal = val.Select(v => System.Convert.ChangeType(v, desType)).ToArray();
            ParamConvertUtil.StoreToParams(context, c, newVal);
        }
    }
}
