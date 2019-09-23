using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Tim.SqlEngine.Models
{
    public class Segment
    {
        public Segment(int number)
        {
            this.Number = number;
        }
        public int Number { get; set; }

        public string Token { get; set; }
        public Match Start { get; internal set; }
        public Match End { get; internal set; }
        public IEnumerable<string> Args { get; internal set; }
        public string ArgContext { get; internal set; }

        public ICollection<Segment> Segments = new List<Segment>();
    }
}
