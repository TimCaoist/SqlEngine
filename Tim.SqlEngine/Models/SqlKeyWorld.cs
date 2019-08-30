using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tim.SqlEngine.Models
{
    public class SqlKeyWorld
    {
        /// <summary>
        /// If 片段
        /// </summary>
        public const string If = "if";

        /// <summary>
        /// Where in 片段
        /// </summary>
        public const string In = "in";

        /// <summary>
        /// 模板
        /// </summary>
        public const string Temp = "temp";

        /// <summary>
        /// 拼接
        /// </summary>
        public const string Join = "join";

        /// <summary>
        /// 切换
        /// </summary>
        public const string Switch = "sw";

        public const string Less = "l";

        public const string LessEqulas = "l=";

        public const string GreatEqulas = "g=";

        public const string Great = "g";

        public const string NotEqulas = "!=";

        public const string Equlas = "==";

        public const string ParamStart = "@";

        public const string GlobalStart = "@g_";

        public const string ObjectStart = "@v_";

        public const string ParentObjectStart = "@v_parent.";

        public const string ContentObjectStart = "@v_content.";

        public const char Split = ':';

        public const char Split3 = ',';

        public const char Split4 = '$';

        public const string Split1 = ",";

        public const char Split2 = '\'';

        public const string ParnetKey = "parent";

        public const string ComplexData = "complex_data";

        public readonly static string Id = "Id";

        public readonly static string ReturnKey = "ReturnKey";

        public const string ComplexDataObjectStart = "@v_cd.";

    }
}
