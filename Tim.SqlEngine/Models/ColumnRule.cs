using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Tim.SqlEngine.Models
{
    [DataContract]
    public class ColumnRule
    {
        [DataMember(Name = "name")]
        public string Name { get; set; }

        [DataMember(Name = "value_type")]
        public ColumnValueType ValueType{ get; set; }

        [DataMember(Name = "value")]
        public object Value { get; set; }

        /// <summary>
        /// 当检查出错时候提示
        /// </summary>
        [DataMember(Name = "error")]
        public string Error { get; internal set; }
    }
}
