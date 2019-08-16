using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tim.SqlEngine.Models;

namespace Tim.SqlEngine.Parser.SegmentBuilder
{
    public static class SegmentUtil
    {
        public static string BuildSql(Context context, string oldSql, Segment segment)
        {
            switch (segment.Token.ToLower())
            {
                case SqlKeyWorld.If:
                    return IfSegmentBuilder.BuildSql(context, oldSql, segment);
            }

            return string.Empty;
        }
    }
}
