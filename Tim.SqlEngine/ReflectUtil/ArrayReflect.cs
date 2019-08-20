using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Tim.SqlEngine.ReflectUtil
{
    public class ArrayReflect : IArrayReflect
    {
        public readonly static ArrayReflect Instance = new ArrayReflect();

        public void SetFiled(object target, IEnumerable<object> datas, PropertyInfo property)
        {
            Array array;
            if (datas.Any() == false)
            {
                array = Array.CreateInstance(property.PropertyType.GetElementType(), 0);
            }
            else {
                var dCount = datas.Count();
                array = Array.CreateInstance(property.PropertyType.GetElementType(), datas.Count());
                Array.Copy(datas.ToArray(), array, dCount);
            }

            property.SetValue(target, array);
        }
    }
}
