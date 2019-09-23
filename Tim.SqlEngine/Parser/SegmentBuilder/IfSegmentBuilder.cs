using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tim.SqlEngine.Common;
using Tim.SqlEngine.Models;

namespace Tim.SqlEngine.Parser.SegmentBuilder
{
    /// <summary>
    /// sample: <if sample=true></>
    /// </summary>
    public static class IfSegmentBuilder
    {
        internal static string BuildSql(IContext context, string oldSql, Segment segment)
        {
            var eval = EvalHelper.GetDelegate(context, segment.ArgContext);
            var result = (bool)eval.DynamicInvoke();
            if (!result)
            {
                return string.Empty;
            }

            var content = SegmentUtil.GetContent(oldSql, segment);
            return SegmentUtil.BuildContent(context, oldSql, content, segment);
        }
    }
}
