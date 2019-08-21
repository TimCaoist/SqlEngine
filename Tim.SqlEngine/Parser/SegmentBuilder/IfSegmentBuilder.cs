using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tim.SqlEngine.Models;

namespace Tim.SqlEngine.Parser.SegmentBuilder
{
    public static class IfSegmentBuilder
    {
        internal static string BuildSql(IContext context, string oldSql, Segment segment)
        {
            var queryParams = context.Params;
            var args = segment.Args;
            var result = IsMatch(context, args);
            if (!result)
            {
                return string.Empty;
            }

            var content = SegmentUtil.GetContent(oldSql, segment);
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

        private static bool IsMatch(IContext context, IEnumerable<string> args)
        {
            string params1 = ParamsUtil.GetParamData(context, args.ElementAt(0)).Data.ToString();
            string params2 = ParamsUtil.GetParamData(context, args.ElementAt(2)).Data.ToString();

            switch (args.ElementAt(1))
            {
                case SqlKeyWorld.Less:
                    return double.Parse(params1) < double.Parse(params2);
                case SqlKeyWorld.LessEqulas:
                    return double.Parse(params1) <= double.Parse(params2);
                case SqlKeyWorld.Great:
                    return double.Parse(params1) > double.Parse(params2);
                case SqlKeyWorld.GreatEqulas:
                    return double.Parse(params1) >= double.Parse(params2);
                case SqlKeyWorld.Equlas:
                    return params2 == params1.ToString();
                case SqlKeyWorld.NotEqulas:
                    return params2 != params1.ToString();
            }

            return false;
        }
    }
}
