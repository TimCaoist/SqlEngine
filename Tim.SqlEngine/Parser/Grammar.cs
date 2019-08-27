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
        private readonly List<Match> matches = new List<Match>();

        private readonly string sql;

        public Grammar(MatchCollection matches, string sql)
        {
            foreach (Match item in matches)
            {
                this.matches.Add(item);
            }
            
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

            Segment segment = null;
            while (matches.Any())
            {
                segment = Calculation(segment != null ? (segment.Number + 1) : 0);
                SetParent(segment, segments);
                matches.Remove(segment.Start);
                matches.Remove(segment.End);
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

        private Segment Calculation(int number)
        {
            var c = 0;
            Segment segment = null;
            var mCount = matches.Count;
            for (var i = 0; i < mCount; i++)
            {
                var match = matches[i];
                var text = match.ToString();
                if (!text.StartsWith(string.Intern("</")))
                {
                    if (segment == null)
                    {
                        var strs = text.Replace(string.Intern("<"), string.Empty).Replace(string.Intern(">"), string.Empty).Split(new char[] { ' ', ':'});
                        segment = new Segment(number)
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
