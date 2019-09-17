using System.Linq;
using Tim.SqlEngine.Models;
using Tim.SqlEngine.ValueSetter;

namespace Tim.SqlEngine.Parser.SegmentBuilder
{
    internal static class LimitSegmentBuilder
    {
        internal static string BuildSql(IContext context, string oldSql, Segment segment)
        {
            var data1 = ParamsUtil.GetParamData(context, segment.Args.ElementAt(0)).Data;
            var data2 = ParamsUtil.GetParamData(context, segment.Args.ElementAt(1)).Data;
            string start = ValueGetter.Builder(data1, false);
            string size = ValueGetter.Builder(data2, false);
            return string.Concat(string.Intern("limit "), start, SqlKeyWorld.Split3, size);
        }
    }
}
