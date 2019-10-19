using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Tim.SqlEngine.Parser;

namespace Tim.SqlEngine.ReflectUtil
{
    public static class ReflectUtil
    {
        private readonly static string GetEnumerator = "GetEnumerator";

        private readonly static Dictionary<string, WeakReference<Assembly>> Assemblies = new Dictionary<string, WeakReference<Assembly>>();

        private const string ArrayTypeEnd = "[]";

        public static bool IsArray(object data) {
            var type = data.GetType();
            if (type == typeof(string))
            {
                return false;
            }

            return type.GetMethod(GetEnumerator) != null;
        }

        private static Assembly GetAssembly(string assemblyString)
        {
            WeakReference<Assembly> weakReference;
            Assembly assembly;
            if (Assemblies.TryGetValue(assemblyString, out weakReference))
            {
                if (weakReference.TryGetTarget(out assembly))
                {
                    if (assembly == null)
                    {
                        assembly = Assembly.Load(assemblyString);
                        weakReference.SetTarget(assembly);
                    }
                }
            }
            else
            {
                assembly = Assembly.Load(assemblyString);
                weakReference = new WeakReference<Assembly>(assembly);
                Assemblies.Add(assemblyString, weakReference);
            }

            return assembly;
        }

        public static object CreateInstance(string assemblyString, string typeStr)
        {
            Assembly assembly = GetAssembly(assemblyString);
            var isArray = typeStr.EndsWith(ArrayTypeEnd);
            if (isArray)
            {
                var instanceType = assembly.GetType(typeStr.Substring(0, typeStr.Length - ArrayTypeEnd.Length), true);
                return Array.CreateInstance(instanceType, 0);
            }

            return assembly.CreateInstance(typeStr, true);
        }

        public static Type CreateType(string assemblyString, string typeStr)
        {
            Assembly assembly = GetAssembly(assemblyString);
            var isArray = typeStr.EndsWith(ArrayTypeEnd);
            if (isArray)
            {
                var instanceType = assembly.GetType(typeStr.Substring(0, typeStr.Length - ArrayTypeEnd.Length), true);
                return Array.CreateInstance(instanceType, 0).GetType();
            }

            return assembly.GetType(typeStr, true);
        }

        public static object GetProperty(object data, string field)
        {
            IDictionary<string, object> dictObj = data as IDictionary<string, object>;
            if (dictObj != null)
            {
                object val;
                if (!dictObj.TryGetValue(field, out val))
                {
                    return string.Empty;
                }

                return val;
            }

            var itemType = data.GetType();
            var ps = itemType.GetProperty(field);
            if (ps == null)
            {
                throw new ArgumentException(string.Concat(field, "不存在"));
            }

            return ps.GetValue(data);
        }

        public static void SetProperty(object data, string field, object fieldVal)
        {
            var instanceType = data.GetType();
            var property = instanceType.GetProperty(field);
            if (property == null)
            {
                return;
            }

            if (property.PropertyType == fieldVal.GetType())
            {
                property.SetValue(data, fieldVal);
                return;
            }

            IArrayReflect arrayReflect = ArrayReflectCreator.Create(fieldVal, property.PropertyType);
            arrayReflect.SetFiled(data, (IEnumerable<object>)fieldVal, property);
        }

        public static IEnumerable<Type> GetSubTypes(Type type)
        {
            var types = type.Assembly.GetTypes();
            ICollection<Type> returnTypes = new List<Type>();
            foreach (var item in types)
            {
                if (item.IsAbstract)
                {
                    continue;
                }

                if (type.IsAbstract && type.IsInterface == false)
                {
                    if (!item.IsSubclassOf(type))
                    {
                        continue;
                    }
                }
                else {
                    if (item.GetInterface(type.FullName) == null)
                    {
                        continue;
                    }
                }

                returnTypes.Add(item);
            }

            return returnTypes;
        }
    }
}
