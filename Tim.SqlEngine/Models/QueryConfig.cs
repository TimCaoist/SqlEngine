﻿using Newtonsoft.Json.Linq;
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
        /// <summary>
        /// 是否合并字段
        /// </summary>
        [DataMember(Name = "megre")]
        public bool Megre { get; set; } = true;

        /// <summary>
        /// 是否只有一条记录
        /// </summary>
        [DataMember(Name = "one")]
        public bool OnlyOne { get; set; }

        /// <summary>
        /// 返回值设置的字段
        /// </summary>
        [DataMember(Name = "field")]
        public string Filed { get;  set; }

        /// <summary>
        /// 不填充该字段
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
