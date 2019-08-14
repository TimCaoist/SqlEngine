using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Tim.SqlEngine.Models
{
    [DataContract]
    public class HandlerConfig : BaseHadlerConfig
    {
        [DataMember(Name = "name")]
        public string Name { get; set; }


        [DataMember(Name = "configs")]
        public IEnumerable<QueryConfig> Configs { get; set; }

        
    }
}
