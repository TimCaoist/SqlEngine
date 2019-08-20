using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tim.SqlEngine.ReflectUtil
{
    public static class ArrayReflectCreator
    {
        public static IArrayReflect Create(object fieldVal, Type propertyType)
        {
            var isArray = ReflectUtil.IsArray(fieldVal);
            if (isArray == false)
            {
                throw new ArgumentException("类型不匹配!");
            }

            var type = propertyType;
            if (type.IsArray)
            {
                return ArrayReflect.Instance;
            }

            if (type.Name.EndsWith("List`1") || type.Name.EndsWith("IEnumerable`1"))
            {
                return ListReflect.Instance;
            }

            throw new ArgumentException("未找到对应类型处理器");
        }
    }
}
