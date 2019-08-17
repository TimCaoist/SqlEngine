using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;
using Tim.SqlEngine.Models;
using Context = Tim.SqlEngine.Models.Context;

namespace Tim.SqlEngine.Parser.SegmentBuilder
{
    public static class InSegmentBuilder
    {
        internal static string BuildSql(Context context, string oldSql, Segment segment)
        {
            string inParams = ParamsUtil.GetParamData(context, segment.Args.ElementAt(0)).Data.ToString();

            if (!string.IsNullOrEmpty(inParams))
            {
                return string.Concat(segment.Args.ElementAt(0), " ", SqlKeyWorld.In, "(", inParams, ")");
            }

            return SegmentUtil.GetContent(oldSql, segment);
        }
    }
}
