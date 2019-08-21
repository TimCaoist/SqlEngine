using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tim.SqlEngine.Models;

namespace Tim.SqlEngine.SqlHelper.UpdateHandler
{
    public abstract class BaseUpdateHandler : IUpdateHandler
    {
        public abstract int Type { get; }

        public abstract object Update(UpdateContext context);
    }
}
