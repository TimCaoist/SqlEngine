using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tim.SqlEngine.Models
{
    public class Context : BaseContext<Context, HandlerConfig, QueryConfig>, IContext
    {
        public Context() : base(null)
        {
        }

        public Context(Context parent) : base(parent)
        {
        }

    }
}
