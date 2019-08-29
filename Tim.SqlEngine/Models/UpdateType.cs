using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tim.SqlEngine.Models
{
    public enum ActionType {

        Insert = 0,

        Update = 1,

        Delete = 2,

        BothInsertAndUpdate = 3,

        All = 4
    }
    /// <summary>
    /// 类型
    /// </summary>
    public enum UpdateType
    {
        /// <summary>
        /// 插入默认值
        /// </summary>
        DefaultValue = 0,

        /// <summary>
        /// 检查值
        /// </summary>
        CheckValue = 1,
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

    /// <summary>
    /// 列类型
    /// </summary>
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

        /// <summary>
        /// 使用公式
        /// </summary>
        Eval = 11,

        /// <summary>
        /// 在数组里面
        /// </summary>
        IsIn = 12,

        /// <summary>
        /// 等于
        /// </summary>
        Equlas = 13
    }

}
