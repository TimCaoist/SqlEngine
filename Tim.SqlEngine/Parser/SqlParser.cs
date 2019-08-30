using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Tim.SqlEngine.Models;
using Tim.SqlEngine.Parser.SegmentBuilder;

namespace Tim.SqlEngine.Parser
{
    public static class SqlParser
    {
        private readonly static string WhiteSpace = " ";

        private readonly static Dictionary<long, WeakReference<string>> Sqls = new Dictionary<long, WeakReference<string>>();

        private static string GetFormatSql(string sql, Func<string, string> getFormatSql)
        {
            WeakReference<string> weakReference;
            string formatSql = string.Empty;
            if (Sqls.TryGetValue(sql.GetHashCode(), out weakReference))
            {
                weakReference.TryGetTarget(out formatSql);
                if (!string.IsNullOrEmpty(formatSql))
                {
                    return formatSql;
                }

                formatSql = getFormatSql(sql);
                weakReference.SetTarget(formatSql);
            }
            else
            {
                formatSql = getFormatSql(sql);
                weakReference = new WeakReference<string>(formatSql);
                Sqls.Add(sql.GetHashCode(), weakReference);
            }

            return formatSql;
        }

        public static Tuple<string, IDictionary<string, object>> Convert(IContext context, string sql)
        {
            sql = sql.Insert(sql.Length, WhiteSpace);
            var matches = SegmentUtil.GetMatch(sql);
            if (matches.Count == 0)
            {
                return GetApplyParamRuleSql(context, sql);
            }

            Func<string, string> getFormatSql = (argSql) =>
            {
                var grammar = new Grammar(matches, argSql);
                IEnumerable<Segment> segments = grammar.Parser();
                if (segments.Any())
                {
                    argSql = GetApplyGramarRuleSql(context, argSql, segments);
                }

                return argSql;
            };

            var newSql = GetFormatSql(sql, getFormatSql);
            return GetApplyParamRuleSql(context, newSql);
        }

        public static Tuple<string, IDictionary<string, object>> GetApplyParamRuleSql(IContext context, string sql)
        {
            var matches = Regex.Matches(sql, string.Intern("@.*?[, ]"));
            var usedParams = ParamsUtil.GetParams(context, matches);
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
