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
        public static string BuildSql(string oldSql, Segment segment, IDictionary<string, object> queryParams)
        {
            switch (segment.Token.ToLower())
            {
                case SqlKeyWorld.If:
                    return IfSegmentBuilder.BuildSql(oldSql, segment, queryParams);
            }

            return string.Empty;
        }
    }
}
