using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tim.SqlEngine.Models
{
    public class SqlKeyWorld
    {
        public const string If = "if";

        public const string In = "in";

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

        public const char Split = ':';

        public const string Split1 = ",";

        public const char Split2 = '\'';

        public const string ParnetKey = "parent";
    }
}
