using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tim.SqlEngine.Models;
using Tim.SqlEngine.ValueSetter;

namespace Tim.SqlEngine.SqlHelper.QueryHandler.ReleatedFillHandler
{
    public class SingleRecodFillHandler : IFillHandler
    {
        public readonly static SingleRecodFillHandler Instance = new SingleRecodFillHandler();

        public object Fill(ReleatedQuery config, object parent, IEnumerable<object> matchDatas, IValueSetter valueSetter)
        {
            var data = matchDatas.FirstOrDefault();
            valueSetter.SetField(parent, data, config.Filed);
            return data;
        }
    }
}
