using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Tim.SqlEngine.Models;
using Tim.SqlEngine.Parser.SegmentBuilder;

namespace Tim.SqlEngine.Parser
{
    public static class SqlParser
    {
        private readonly static string SegmentStr = "<.*?>";

        public static Tuple<string, IDictionary<string, object>> Convert(IContext context, string sql)
        {
            sql = sql.Insert(sql.Length, " ");
            var matches = Regex.Matches(sql, SegmentStr);
            if (matches.Count > 0)
            {
                var grammar = new Grammar(matches, sql);
                IEnumerable<Segment> segments = grammar.Parser();
                if (segments.Any())
                {
                    sql = ApplyGramar(context, sql, segments);
                }
            }
            matches = Regex.Matches(sql, string.Intern("@.*? |@.*,"));
            var usedParams = ParamsUtil.GetParams(context, matches);
            sql = ParamsUtil.ApplyParams(sql, usedParams);
            return Tuple.Create(sql, ParamsUtil.Convert(usedParams));
        }

        public static string ApplyGramar(IContext contex, string sql, IEnumerable<Segment> segments)
        {
            var oldSql = sql.Clone().ToString();
            var total = segments.Count();
            for (var i = total - 1; i >= 0; i--)
            {
                var seg = segments.ElementAt(i);
                var startIndex = seg.Start.Index;
                var endIndex = seg.End.Index + seg.End.Length;
                sql = sql.Remove(startIndex, endIndex - startIndex);
                sql = sql.Insert(startIndex, SegmentUtil.BuildSql(contex, oldSql, seg));
            }

            return sql;
        }
    }
}
