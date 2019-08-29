using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Tim.SqlEngine.Models
{
    [DataContract]
    public class UpdateRule
    {
        [DataMember(Name = "update_type")]
        public UpdateType UpdateType { get; set; }

        [DataMember(Name = "range_type")]
        public RangeType RangeType { get; set; }

        [DataMember(Name = "db_keys")]
        public string[] DBKeys { get; set; }

        [DataMember(Name = "tables")]
        public string[] Tables { get; set; }

        [DataMember(Name = "columns")]
        public IEnumerable<ColumnRule> Columns { get; set; }
    }
}
