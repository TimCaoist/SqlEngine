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
    public class HandlerConfig : BaseHadlerConfig, IHandlerConfig
    {
        /// <summary>
        /// 配置名称
        /// </summary>
        [DataMember(Name = "name")]
        public string Name { get; set; }

        /// <summary>
        /// 主查询
        /// </summary>
        [DataMember(Name = "configs")]
        public IEnumerable<QueryConfig> Configs { get; set; }

        /// <summary>
        /// 模板名称
        /// </summary>
        [DataMember(Name = "templates")]

        public IEnumerable<Template> Templates { get; set; }
    }
}
