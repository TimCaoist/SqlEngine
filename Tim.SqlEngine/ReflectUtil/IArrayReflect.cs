using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Tim.SqlEngine.ReflectUtil
{
    public interface IArrayReflect
    {
        void SetFiled(object target, IEnumerable<object> datas, PropertyInfo property);
    }
}
