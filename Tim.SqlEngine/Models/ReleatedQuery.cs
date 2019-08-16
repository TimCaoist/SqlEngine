using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Tim.SqlEngine.Models
{
    [DataContract]
    public class ReleatedQuery : QueryConfig
    {
        [DataMember(Name = "compare_fields")]
        public string[] CompareFields { get; set; }

        [DataMember(Name = "is_single")]
        public bool IsSingle { get; set; }
    }
}
