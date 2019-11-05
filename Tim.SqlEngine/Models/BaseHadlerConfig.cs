using Newtonsoft.Json.Linq;
using System.Runtime.Serialization;
using Tim.CacheUtil.Models;

namespace Tim.SqlEngine.Models
{
    [DataContract]
    public abstract class BaseHadlerConfig
    {
        /// <summary>
        /// 其他配置
        /// </summary>
        [DataMember(Name = "config")]
        public JObject Config { get; set; }

        /// <summary>
        /// 连接字符串
        /// </summary>
        [DataMember(Name = "connection")]
        public string Connection { get; set; }

        /// <summary>
        /// 操作方式
        /// </summary>
        [DataMember(Name = "query_type")]
        public int QueryType { get; set; } = 1;

        /// <summary>
        /// Sql语句
        /// </summary>
        [DataMember(Name = "sql")]
        public string Sql { get; set; }

        /// <summary>
        /// 缓存配置
        /// </summary>
        [DataMember(Name = "cache")]
        public CacheConfig CacheConfig { get; set; }

    }
}
