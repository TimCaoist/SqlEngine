using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tim.SqlEngine.Models;

namespace Tim.SqlEngine.ValueSetter
{
    public static class ValueSetterCreater
    {
        public static IValueSetter Create(this BaseHadlerConfig queryConfig)
        {
            IValueSetter valueSetter;
            if (queryConfig.Config == null || 
                queryConfig.Config["assembly_str"] == null || 
                queryConfig.Config["type_str"] == null)
            {
                valueSetter = new DynamicValueSetter();
            }
            else
            {
                valueSetter = new ReflectValueSetter(queryConfig.Config["assembly_str"].ToString(), queryConfig.Config["type_str"].ToString());
            }

            return valueSetter;
        }
    }
}
