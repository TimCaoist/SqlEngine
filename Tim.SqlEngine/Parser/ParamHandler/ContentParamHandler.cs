using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tim.SqlEngine.Models;

namespace Tim.SqlEngine.Parser.ParamHandler
{
    internal class ContentParamHandler : ParentParamHandler
    {
        public override bool Match(string paramStr)
        {
            return paramStr.StartsWith(SqlKeyWorld.ContentObjectStart, StringComparison.OrdinalIgnoreCase);
        }

        protected override object GetObject(string objectKey, IContext context)
        {
            return context.Data;
        }
    }
}
