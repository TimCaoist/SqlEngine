using Newtonsoft.Json.Linq;
using System.Runtime.Serialization;
using Tim.CacheUtil.Models;

namespace Tim.SqlEngine.Models
{
    [DataContract]
    public abstract class BaseHadlerConfig
    {
        [DataMember(Name = "config")]
        public JObject Config { get; set; }

        [DataMember(Name = "connection")]
        public string Connection { get; set; }

        [DataMember(Name = "query_type")]
        public int QueryType { get; set; } = 1;

        [DataMember(Name = "sql")]
        public string Sql { get; set; }

        [DataMember(Name = "cache")]
        public CacheConfig CacheConfig { get; set; }

    }
}
