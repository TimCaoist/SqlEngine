using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Tim.SqlEngine.Models
{
    [DataContract]
    public class Template
    {
        /// <summary>
        /// 模板名称
        /// </summary>
        [DataMember(Name = "name")]
        public string Name { get; set; }

        /// <summary>
        /// 模板值
        /// </summary>

        [DataMember(Name = "value")]
        public string Value { get; set; }
    }
}
