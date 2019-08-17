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
        /// 是否只使用查询到的首条记录
        /// </summary>
        [DataMember(Name = "is_single")]
        public bool IsSingle { get; set; }

        /// <summary>
        /// 只使用查询到的某个字段
        /// </summary>
        [DataMember(Name = "used_field")]
        public string UsedField { get; set; }

        /// <summary>
        /// 只使用查询到的某个字段的缺省值
        /// </summary>
        [DataMember(Name = "used_field_default_value")]
        public object UsedFieldDefaultValue { get; set; }
    }
}
