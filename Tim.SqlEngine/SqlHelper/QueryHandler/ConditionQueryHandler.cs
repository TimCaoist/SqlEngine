using Newtonsoft.Json;
using System;
using Tim.SqlEngine.Common;
using Tim.SqlEngine.Models;

namespace Tim.SqlEngine.SqlHelper.QueryHandler
{
    public class ConditionQueryHandler : BaseQueryHandler, IConditionQueryHandler
    {
        public override int Type => 6;

        private readonly static string ConditionSetting = "setting";

        private ConditionModel GetConditionModel(Context context)
        {
            object model;
            if (context.ContentParams.TryGetValue(ConditionSetting, out model)) {
                return (ConditionModel)model;
            }

            var queryConfig = context.Config;
            var conditionSetting = JsonConvert.DeserializeObject<ConditionModel>(queryConfig.Config[ConditionSetting].ToString());
            context.ContentParams.Add(ConditionSetting, conditionSetting);
            return conditionSetting;
        }

        public bool Continue(Context context)
        {
            var conditionSetting = GetConditionModel(context);
            var @delegate = EvalHelper.GetDelegate(context, conditionSetting.Eval);
            return (bool)@delegate.DynamicInvoke();
        }

        public bool WhetheStop(Context context)
        {
            var conditionSetting = GetConditionModel(context);
            return conditionSetting.StopByEval;
        }

        protected override object DoQuery(Context context)
        {
            var conditionSetting = GetConditionModel(context);
            IQueryHandler queryHandler = QueryHandlerFactory.GetQueryHandler(conditionSetting.QueryType);
            return queryHandler.Query(context);
        }

        public bool WhetheResultStop(Context context, object result)
        {
            var conditionSetting = GetConditionModel(context);
            if (string.IsNullOrEmpty(conditionSetting.ResultEval))
            {
                return false;
            }

            var @delegate = EvalHelper.GetDelegate(context, conditionSetting.ResultEval, result);
            return (bool)@delegate.DynamicInvoke(result);
        }
    }
}
