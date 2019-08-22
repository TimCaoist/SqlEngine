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
    public class QueryConfig : BaseHadlerConfig
    {

        [DataMember(Name = "one")]
        public bool OnlyOne { get; set; }

        /// <summary>
        /// 返回值设置的字段
        /// </summary>
        [DataMember(Name = "field")]
        public string Filed { get;  set; }

        /// <summary>
        /// 忽略填充
        /// </summary>
        [DataMember(Name = "ingore_fill")]
        public bool IngoreFill { get; set; }

        /// <summary>
        /// 将查询的字段重命名
        /// </summary>
        [DataMember(Name = "alais")]
        public string[] Alais { get; set; }
    }
}
