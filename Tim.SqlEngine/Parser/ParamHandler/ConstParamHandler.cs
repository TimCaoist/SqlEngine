using Tim.SqlEngine.Models;

namespace Tim.SqlEngine.Parser.ParamHandler
{
    internal class ConstParamHandler : IParamHandler
    {
        public ParamInfo GetParamInfo(IContext context, string dataStr)
        {
            return new ParamInfo
            {
                Type = ParamType.Constant,
                Name = string.Empty,
                Data = dataStr
            };
        }

        public bool Match(string paramStr)
        {
            return !paramStr.StartsWith(SqlKeyWorld.ParamStart);
        }
    }
}
