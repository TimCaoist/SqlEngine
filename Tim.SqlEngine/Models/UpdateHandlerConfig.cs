using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Tim.SqlEngine.Models
{
    [DataContract]
    public class UpdateHandlerConfig : BaseHadlerConfig, IHandlerConfig
    {
        /// <summary>
        /// 模板名称
        /// </summary>
        [DataMember(Name = "templates")]

        public IEnumerable<Template> Templates { get; set; }

        /// <summary>
        /// 主查询
        /// </summary>
        [DataMember(Name = "configs")]
        public IEnumerable<UpdateConfig> Configs { get; set; }

        /// <summary>
        /// json提交的类型
        /// </summary>
        [DataMember(Name = "json_type")]
        public string JType { get; set; }

        [DataMember(Name = "param_converts")]
        public IEnumerable<ParamConvertConfig> ParamConfigs { get; set; }
    }
}
