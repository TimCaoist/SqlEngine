using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Tim.SqlEngine.Models;
using Tim.SqlEngine.Parser.SegmentBuilder;
using Tim.SqlEngine.Common;

namespace Tim.SqlEngine.Parser
{
    public static class SqlParser
    {
        private readonly static Dictionary<long, WeakReference<string>> Sqls = new Dictionary<long, WeakReference<string>>();

        public static string GetFormatSql(IContext context, string sql) {
            sql = sql.Insert(sql.Length, SqlKeyWorld.WhiteSpace);
            var matches = SegmentUtil.GetMatch(sql);
            if (matches.Count == 0)
            {
                return sql;
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

            var newSql = getFormatSql(sql);
            return newSql;
        }

        public static Tuple<string, IDictionary<string, object>> Convert(IContext context, string sql)
        {
            sql = GetFormatSql(context, sql);
            return Tuple.Create(sql, ParamsUtil.GetParams(context, sql).Item1.ParamsToDictionary());
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
