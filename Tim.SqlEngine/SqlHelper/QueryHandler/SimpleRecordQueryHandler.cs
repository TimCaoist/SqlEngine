using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tim.SqlEngine.Models;
using Tim.SqlEngine.ValueSetter;

namespace Tim.SqlEngine.SqlHelper.QueryHandler
{
    public class SimpleRecordQueryHandler : BaseQueryHandler
    {
        public override int Type => 4;

        public override object Query(Context context)
        {
            var queryConfig = context.Config;
            var handlerConfig = context.HandlerConfig;
            if (string.IsNullOrEmpty(queryConfig.Connection))
            {
                queryConfig.Connection = handlerConfig.Connection;
            }

            IValueSetter valueSetter = new ValueSetter.SimpleRecordQueryHandler();
            var datas = SqlQueryExcuter.ExcuteQuery(context, valueSetter);
            context.Data = datas;
            return datas;
        }
    }
}
