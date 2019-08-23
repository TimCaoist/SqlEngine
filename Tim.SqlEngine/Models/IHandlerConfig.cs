using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tim.SqlEngine.Models
{
    public interface IHandlerConfig
    {
        IEnumerable<Template> Templates { get; set; }
    }
}
