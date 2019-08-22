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
        /// <summary>
        /// 需要比较的字段格式为[父字段]:[子字段]
        /// </summary>
        [DataMember(Name = "compare_fields")]
        public string[] CompareFields { get; set; }

        /// <summary>
        /// 填充的字段
        /// </summary>
        [DataMember(Name = "fill_fields")]
        public string[] FillFields { get; set; }

        /// <summary>
        /// 填充字段默认值
        /// </summary>
        [DataMember(Name = "default_values")]
        public object[] DefaultValues { get; set; } = new object[] { };

        /// <summary>
        /// 字段赋值方式
        /// 0（默认）以数组形式赋值
        /// 1 单个对象赋值
        /// 2 多个字段赋值
        /// </summary>
        [DataMember(Name = "fill_type")]
        public int FillType { get; set; }

        /// <summary>
        /// 值默认为true 当为true的时候匹配过一次的数据将被移除
        /// </summary>
        [DataMember(Name = "match_one_time")]
        public bool MatchOneTime { get; set; } = true;
    }
}
