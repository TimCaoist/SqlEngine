﻿using System;
using System.Collections.Generic;
using Tim.SqlEngine.Models;

namespace Tim.SqlEngine.Parser.ParamHandler
{
    public class ParentParamHandler : ObjectParamHandler
    {
        public override bool Match(string paramStr)
        {
            return paramStr.StartsWith(SqlKeyWorld.ParentObjectStart, StringComparison.OrdinalIgnoreCase);
        }

        protected override object GetObject(string objectKey, IContext context)
        {
            return context.GetParent().Data;
        }

        protected override IDictionary<string, object> GetQueryParams(IContext context)
        {
            return context.ContentParams;
        }
    }
}
