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
        private readonly static string WhiteSpace = " ";

        private readonly static string LessEqulas = "<=";

        private readonly static string Less = "< ";

        public static Tuple<string, IDictionary<string, object>> Convert(IContext context, string sql)
        {
            sql = sql.Insert(sql.Length, WhiteSpace);
            var matches = SegmentUtil.GetMatch(sql);
            if (matches.Count == 0)
            {
                return GetApplyParamRuleSql(context, sql);
            }

            var grammar = new Grammar(matches, sql);
            IEnumerable<Segment> segments = grammar.Parser();
            if (segments.Any())
            {
                sql = GetApplyGramarRuleSql(context, sql, segments);
            }

            return GetApplyParamRuleSql(context, sql);
        }

        public static Tuple<string, IDictionary<string, object>> GetApplyParamRuleSql(IContext context, string sql)
        {
            var matches = Regex.Matches(sql, string.Intern("@.*? |@.*,"));
            var usedParams = ParamsUtil.GetParams(context, matches);
            sql = ParamsUtil.ApplyParams(sql, usedParams);
            return Tuple.Create(sql, ParamsUtil.Convert(usedParams));
        }

        public static string GetApplyGramarRuleSql(IContext contex, string sql, IEnumerable<Segment> segments)
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
