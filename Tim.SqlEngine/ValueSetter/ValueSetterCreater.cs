using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tim.SqlEngine.Common;
using Tim.SqlEngine.Models;

namespace Tim.SqlEngine.ValueSetter
{
    public static class ValueSetterCreater
    {
        internal const string TypeStr = "type_str";

        public static IValueSetter Create(this BaseHadlerConfig queryConfig)
        {
            IValueSetter valueSetter;
            if (queryConfig.Config == null || 
                string.IsNullOrEmpty(queryConfig.Config[TypeStr].ToSingleData<string>(string.Empty)))
            {
                valueSetter = new DynamicValueSetter();
            }
            else
            {
                var typeStr = queryConfig.Config[TypeStr].ToSingleData<string>().Split(SqlKeyWorld.Split3);
                valueSetter = new ReflectValueSetter(typeStr[0], typeStr[1]);
            }

            return valueSetter;
        }

        public static IValueSetter Create(object data)
        {
            IValueSetter valueSetter;
            if (data is IDictionary<string, object>)
            {
                valueSetter = new DynamicValueSetter();
            }
            else
            {
                valueSetter = new ReflectValueSetter();
            }

            return valueSetter;
        }
    }
}
