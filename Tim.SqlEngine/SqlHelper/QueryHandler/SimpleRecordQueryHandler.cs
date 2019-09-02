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

        protected override object DoQuery(Context context)
        {
            var queryConfig = context.Config;
            var handlerConfig = context.HandlerConfig;
            if (string.IsNullOrEmpty(queryConfig.Connection))
            {
                queryConfig.Connection = handlerConfig.Connection;
            }

            IValueSetter valueSetter = new ValueSetter.SimpleRecordQueryHandler();
            var datas = SqlExcuter.ExcuteQuery(context, valueSetter);
            context.Data = datas;
            return datas;
        }
    }
}
