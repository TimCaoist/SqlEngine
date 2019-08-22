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
            var args = segment.Args;
            var result = IsMatch(context, args);
            if (!result)
            {
                return string.Empty;
            }

            var content = SegmentUtil.GetContent(oldSql, segment);
            return SegmentUtil.BuildContent(context, oldSql, content, segment);
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
