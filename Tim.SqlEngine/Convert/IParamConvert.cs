using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tim.SqlEngine.Models;

namespace Tim.SqlEngine.Convert
{
    public interface IParamConvert
    {
        ParamConvertType ConvertType { get;}

        void DoConvert(ParamConvertConfig c, IContext context);
    }
}
