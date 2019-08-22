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
        public static string GetContent(string sql, Segment segment)
        {
            var parentStatIndex = segment.Start.Index + segment.Start.Length;
            var parentEndIndex = segment.End.Index;
            var content = sql.Substring(parentStatIndex, parentEndIndex - parentStatIndex);
            return content;
        }

        public static string BuildSql(IContext context, string oldSql, Segment segment)
        {
            switch (segment.Token.ToLower())
            {
                case SqlKeyWorld.If:
                    return IfSegmentBuilder.BuildSql(context, oldSql, segment);
                case SqlKeyWorld.In:
                    return InSegmentBuilder.BuildSql(context, oldSql, segment);
                case SqlKeyWorld.Temp:
                    return TempSegmentBuilder.BuildSql(context, oldSql, segment);
                case SqlKeyWorld.Switch:
                    return TempSegmentBuilder.BuildSql(context, oldSql, segment);
                case SqlKeyWorld.Join:
                    return JoinSegmentBuilder.BuildSql(context, oldSql, segment);
            }

            return string.Empty;
        }

        public static string BuildContent(IContext context, string oldSql, string content, Segment segment)
        {
            if (segment.Segments.Any() == false)
            {
                return content;
            }

            var parentStatIndex = segment.Start.Index + segment.Start.Length;
            var parentEndIndex = segment.End.Index;

            var total = segment.Segments.Count();
            for (var i = total - 1; i >= 0; i--)
            {
                var seg = segment.Segments.ElementAt(i);
                var startIndex = seg.Start.Index - parentStatIndex;
                var endIndex = seg.End.Index + seg.End.Length - parentStatIndex;
                content = content.Remove(startIndex, endIndex - startIndex);
                content = content.Insert(startIndex, SegmentUtil.BuildSql(context, oldSql, seg));
            }

            return content;
        }
    }
}
