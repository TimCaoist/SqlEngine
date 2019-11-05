using System.Collections.Generic;
using System.Linq;
using Tim.SqlEngine.Models;
using Tim.SqlEngine.ValueSetter;

namespace Tim.SqlEngine.SqlHelper.QueryHandler.ReleatedFillHandler
{
    public class FieldsFillHandler : IFillHandler
    {
        public readonly static FieldsFillHandler Instance = new FieldsFillHandler();

        public object Fill(ReleatedQuery config, object parent, IEnumerable<object> matchDatas, IValueSetter valueSetter)
        {
            var data = matchDatas.FirstOrDefault();
            var fieldCount = config.FillFields.Count();
            var dValueCount = config.DefaultValues.Count();

            if (data == null)
            {
                for (var i = 0; i < fieldCount; i++)
                {
                    var field = config.FillFields.ElementAt(i);
                    if (i >= dValueCount)
                    {
                        break;
                    }

                    var val = config.DefaultValues.ElementAt(i);
                    valueSetter.SetField(parent, val, field);
                }

                return null;
            }

            for (var i = 0; i < fieldCount; i++)
            {
                var field = config.FillFields.ElementAt(i);
                var val = ValueGetter.GetValue(field, data).Data;
                valueSetter.SetField(parent, val, field);
            }

            return null;
        }
    }
}
