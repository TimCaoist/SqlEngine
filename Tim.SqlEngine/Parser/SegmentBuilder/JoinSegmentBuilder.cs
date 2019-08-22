using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tim.SqlEngine.Models;

namespace Tim.SqlEngine.Parser.SegmentBuilder
{
    public static class JoinSegmentBuilder
    {
        internal static string BuildSql(IContext context, string oldSql, Segment segment)
        {
            var data = ParamsUtil.GetParamData(context, segment.Args.ElementAt(0)).Data;
            IEnumerable<object> datas = data as IEnumerable<object>;
            if (datas == null)
            {
                datas = data.ToString().Split(SqlKeyWorld.Split3);
            }

            var content = SegmentUtil.GetContent(oldSql, segment);
            content = SegmentUtil.BuildContent(context, oldSql, content, segment);
            var paramStrs = content.Trim().Split(SqlKeyWorld.Split4);
            var lenData = datas.Count();
            var template = paramStrs[0];
            var split = string.Empty;
            if (paramStrs.Length > 1)
            {
                split = paramStrs[1];
            }

            StringBuilder sb = new StringBuilder();
            for (var i = 0; i < lenData; i++)
            {
                sb.Append(string.Format(template, datas.ElementAt(i)));
                if (i != lenData - 1)
                {
                    sb.Append(split);
                }
            }

            return sb.ToString();
        }
    }
}
