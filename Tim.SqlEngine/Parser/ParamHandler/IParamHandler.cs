using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tim.SqlEngine.Models;

namespace Tim.SqlEngine.Parser.ParamHandler
{
    public interface IParamHandler
    {
        bool Match(string paramStr);
        ParamInfo GetParamInfo(IContext context, string dataStr);
    }
}
