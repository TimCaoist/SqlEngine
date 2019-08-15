using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tim.SqlEngine.Models;

namespace Tim.SqlEngine.Parser
{
    public class AlaisParser
    {
        private IEnumerable<string> columns;

        private string[] alias;

        private Dictionary<string, string> realAlias;

        public AlaisParser(IEnumerable<string> columns, string[] alias)
        {
            this.columns = columns;
            this.alias = alias ?? new string[] { };
            BuildAlais();
        }

        private void BuildAlais()
        {
            realAlias = new Dictionary<string, string>();
            var colCount = this.columns.Count();
            var aCount = alias.Count();
            for (var i = 0; i < colCount; i++)
            {
                var col = columns.ElementAt(i);
                if (realAlias.ContainsKey(col))
                {
                    continue;
                }

                if (alias.Count() <= i)
                {
                    realAlias.Add(col, col);
                    continue;
                }

                var al = alias[i];
                if (al.IndexOf(SqlKeyWorld.Split) > 0)
                {
                    var aArray = al.Split(SqlKeyWorld.Split);
                    realAlias.Add(aArray[0], aArray[1]);
                }
                else
                {
                    realAlias.Add(col, al);
                }
            }
        }

        internal string GetName(int i, string col)
        {
            var name = string.Empty;
            if (realAlias.TryGetValue(col, out name))
            {
                return name;
            }

            return col;
        }
    }
}
