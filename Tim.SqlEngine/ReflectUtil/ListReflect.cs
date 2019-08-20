using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Tim.SqlEngine.ReflectUtil
{
    public class ListReflect : IArrayReflect
    {
        public readonly static ListReflect Instance = new ListReflect();

        public void SetFiled(object target, IEnumerable<object> datas, PropertyInfo property)
        {
            var list = (System.Collections.IList)Activator.CreateInstance(typeof(List<>).MakeGenericType(property.PropertyType.GenericTypeArguments));
            foreach (var data in datas)
            {
                list.Add(data);
            }

            property.SetValue(target, list);
        }
    }
}
