using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Tim.SqlEngine.Models;

namespace Tim.SqlEngine.Parser
{
    internal class Grammar
    {
        private readonly MatchCollection matches;

        private readonly string sql;

        public Grammar(MatchCollection matches, string sql)
        {
            this.matches = matches;
            this.sql = sql;
        }

        public IEnumerable<Segment> Parser()
        {
            ICollection<Segment> segments = new List<Segment>();
            var mCount = matches.Count;
            if (mCount % 2 != 0 )
            {
                throw new ArgumentException("开始结束语法不匹配!");
            }

            var totalCount = mCount / 2;
            var segment = Calculation(mCount, 0);
            segments.Add(segment);
            while (totalCount > (segment.Number + 1))
            {
                segment = Calculation(mCount, segment.Number + 1);
                SetParent(segment, segments);
            }

            return segments;
        }

        private void SetParent(Segment segment, ICollection<Segment> childs)
        {
            var seg = childs.FirstOrDefault(s => s.Start.Index < segment.Start.Index && segment.End.Index < s.End.Index);
            if (seg == null)
            {
                childs.Add(segment);
                return;
            }

            SetParent(segment, seg.Segments);
        }

        private Segment Calculation(int mCount, int start)
        {
            var c = 0;
            Segment segment = null;
            for (var i = start; i < mCount; i++)
            {
                var match = matches[i];
                var text = match.ToString();
                if (!text.StartsWith(string.Intern("</")))
                {
                    if (segment == null)
                    {
                        var strs = text.Replace(string.Intern("<"), string.Empty).Replace(string.Intern(">"), string.Empty).Split(new char[] { ' ', ':'});
                        segment = new Segment(start)
                        {
                            Start = match,
                            Token = strs[0],
                            Args = strs.Skip(1)
                        };
                    }

                    c++;
                    continue;
                }

                if (segment == null)
                {
                    continue;
                }

                c--;
                if (c == 0)
                {
                    segment.End = match;
                    return segment;
                }
                else if (c < 0 )
                {
                    throw new ArgumentException(sql + "语法错误");
                }
            }

            return segment;
        }
    }
}
