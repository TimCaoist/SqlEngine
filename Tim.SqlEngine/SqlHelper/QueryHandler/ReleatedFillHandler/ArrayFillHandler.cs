using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tim.SqlEngine.Models;
using Tim.SqlEngine.ValueSetter;

namespace Tim.SqlEngine.SqlHelper.QueryHandler.ReleatedFillHandler
{
    public class ArrayFillHandler : IFillHandler
    {
        public readonly static ArrayFillHandler Instance = new ArrayFillHandler();

        public object Fill(ReleatedQuery config, object parent, IEnumerable<object> matchDatas, IValueSetter valueSetter)
        {
            valueSetter.SetField(parent, matchDatas, config.Filed);
            return matchDatas;
        }
    }
}
