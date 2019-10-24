using Tim.SqlEngine.Common;
using Tim.SqlEngine.Models;
using Tim.SqlEngine.ValueSetter;

namespace Tim.SqlEngine.SqlHelper.UpdateHandler
{
    public class CopyDataHandler : BaseUpdateHandler
    {
        public override int Type => 9;

        protected override object DoUpdate(UpdateContext context)
        {
            context.Submit();
            var queryConfig = context.Config;
            var handlerConfig = context.HandlerConfig;
            if (string.IsNullOrEmpty(queryConfig.Connection))
            {
                queryConfig.Connection = handlerConfig.Connection;
            }

            IValueSetter valueSetter = queryConfig.Create();
            var querySql = queryConfig.Config["query_sql"].ToSingleData<string>(string.Empty);
            var datas = SqlExcuter.ExcuteQuery(context, valueSetter, querySql);

            foreach (var data in datas)
            {
                context.ContentParams.ReplaceOrInsert(SqlKeyWorld.ComplexData, data);
                SqlExcuter.ExcuteTrann(context);
            }

            return true;
        }
    }
}
