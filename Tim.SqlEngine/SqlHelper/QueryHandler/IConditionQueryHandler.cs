using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tim.SqlEngine.Models;

namespace Tim.SqlEngine.SqlHelper.QueryHandler
{
    public interface IConditionQueryHandler
    {
        bool Continue(Context context);

        bool WhetheStop(Context context);

        bool WhetheResultStop(Context context, object result);
    }
}
