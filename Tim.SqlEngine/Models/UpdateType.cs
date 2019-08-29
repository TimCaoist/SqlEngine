using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tim.SqlEngine.Models
{
    public enum UpdateType
    {
        /// <summary>
        /// 插入默认值
        /// </summary>
        DefaultValue = 0,
    }

    /// <summary>
    /// 影响范围
    /// </summary>
    public enum RangeType {

        /// <summary>
        /// 表
        /// </summary>
        Tabels = 0,

        /// <summary>
        /// 库
        /// </summary>
        DBs = 1,

        /// <summary>
        /// 所有库和表
        /// </summary>
        All = 2
    }

    public enum ColumnValueType
    {
        /// <summary>
        /// 直接取值
        /// </summary>
        Value = 0,

        /// <summary>
        /// 接口取值
        /// </summary>
        Interface = 1,

        /// <summary>
        /// Func
        /// </summary>
        Func = 2,
    }

}
