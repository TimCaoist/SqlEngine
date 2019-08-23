using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Tim.SqlEngine.Models
{
    [DataContract]
    public class UpdateConfig : BaseHadlerConfig
    {
        [DataMember(Name = "fields")]
        public IEnumerable<string> Fields { get; set; }

        [DataMember(Name ="filter")]
        public string Filter { get; set; }

        [DataMember(Name = "table")]
        public string Table { get; set; }

        [DataMember(Name = "in_tran")]
        public bool InTran { get; set; }

        [DataMember(Name = "return_id")]
        public bool ReturnId { get; set; }
    }
}
