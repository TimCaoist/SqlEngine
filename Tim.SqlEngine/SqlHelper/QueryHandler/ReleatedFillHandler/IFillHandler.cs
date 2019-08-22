using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tim.SqlEngine.Models;

namespace Tim.SqlEngine.SqlHelper.QueryHandler.ReleatedFillHandler
{
    public interface IFillHandler
    {
        object Fill(ReleatedQuery config, object parent, IEnumerable<object> matchDatas, ValueSetter.IValueSetter valueSetter);
    }
}
