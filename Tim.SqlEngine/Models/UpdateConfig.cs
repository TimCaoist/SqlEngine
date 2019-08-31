using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Tim.SqlEngine.Models
{
    [DataContract]
    public class UpdateConfig : BaseHadlerConfig
    {
        /// <summary>
        /// 需要更新的字段集合
        /// 格式1.[字段名称]与数据库对应
        ///     2.[字段名称:对象字段名称]
        /// </summary>
        [DataMember(Name = "fields")]
        public IEnumerable<string> Fields { get; set; }

        /// <summary>
        /// 过滤条件用于Update Delete操作可为空
        /// </summary>
        [DataMember(Name ="filter")]
        public string Filter { get; set; }

        /// <summary>
        /// 操作的表名
        /// </summary>
        [DataMember(Name = "table")]
        public string Table { get; set; }

        [DataMember(Name = "in_tran")]
        public bool InTran { get; set; }

        public bool ReturnId { get; set; }

        /// <summary>
        /// 主键的字段名称如果未设置会先看Id或者id是否存在
        /// 不存在的情况会去数据库取主键
        /// </summary>
        [DataMember(Name = "key")]
        public string Key { get; internal set; }

        /// <summary>
        /// 返回值设置的字段
        /// </summary>
        [DataMember(Name = "field")]
        public string Filed { get; set; }
    }
}
