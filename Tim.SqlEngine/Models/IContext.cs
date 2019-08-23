using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tim.SqlEngine.Models
{
    public interface IContext
    {
        IDictionary<string, object> ContentParams { get; set; }

        IDictionary<string, object> Params { get; set; }

        BaseHadlerConfig GetConfig();

        IHandlerConfig GetHandlerConfig();

        object Data { get; set; }

        IContext GetParent();
    }
}
