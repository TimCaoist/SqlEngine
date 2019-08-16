using System.Collections.Generic;

namespace Tim.SqlEngine.Parser.ParamHandler
{
    public static class ParamHandlerFactory
    {
        private static readonly ICollection<IParamHandler> paramHandlers = new List<IParamHandler>()
        {
            new ConstParamHandler(),
            new GlobalParamHandler(),
            new ParentParamHandler(),
            new ObjectParamHandler(),
            new NormalParamHandler()
        };

        public static IParamHandler Find(string paramStr)
        {
            foreach (var handler in paramHandlers)
            {
                if (handler.Match(paramStr))
                {
                    return handler;
                }
            }

            return null;
        }
    }
}
