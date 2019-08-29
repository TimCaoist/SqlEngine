﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tim.SqlEngine.Models;

namespace Tim.SqlEngine.Parser.SegmentBuilder
{
    public static class TempSegmentBuilder
    {
        internal static string BuildSql(IContext context, string oldSql, Segment segment)
        {
            var templateName = segment.Args.ElementAt(2);
            Template template;
            var index = templateName.LastIndexOf('/');
            if (index < 0)
            {
                template = context.GetHandlerConfig().Templates.FirstOrDefault(t => string.Equals(t.Name, templateName, StringComparison.OrdinalIgnoreCase));
            }
            else {
                var config = templateName.Substring(0, index);
                HandlerConfig handlerConfig = JsonParser.ReadHandlerConfig<HandlerConfig>(config);
                templateName = templateName.Substring(index + 1);
                template = handlerConfig.Templates.FirstOrDefault(t => string.Equals(t.Name, templateName, StringComparison.OrdinalIgnoreCase));
            }

            var content = SegmentUtil.GetContent(oldSql, segment);
            content = SegmentUtil.BuildContent(context, oldSql, content, segment);
            var paramStrs = content.Trim().Split(SqlKeyWorld.Split4);
            return string.Format(template.Value, paramStrs);
        }
    }
}
