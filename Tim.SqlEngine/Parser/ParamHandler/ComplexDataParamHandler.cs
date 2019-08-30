using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tim.SqlEngine.Models;

namespace Tim.SqlEngine.Parser.ParamHandler
{
    public class ComplexDataParamHandler : ObjectParamHandler
    {
        public override bool Match(string paramStr)
        {
            return paramStr.StartsWith(SqlKeyWorld.ComplexDataObjectStart, StringComparison.OrdinalIgnoreCase);
        }

        protected override object GetObject(string objectKey, IContext context)
        {
            var complexData = context.ContentParams[SqlKeyWorld.ComplexData];
            return complexData;
        }

        protected override IDictionary<string, object> GetQueryParams(IContext context)
        {
            return context.ContentParams;
        }
    }
}
