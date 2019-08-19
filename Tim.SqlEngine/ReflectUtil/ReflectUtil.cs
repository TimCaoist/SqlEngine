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

        public static bool IsArray(object data) {
            var type = data.GetType();
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
            return assembly.CreateInstance(typeStr);
        }

        public static Type CreateType(string assemblyString, string typeStr)
        {
            Assembly assembly = GetAssembly(assemblyString);
            return assembly.GetType(typeStr);
        }

        public static object GetProperty(object data, string field)
        {
            var itemType = data.GetType();
            IDictionary<string, object> dictObj = data as IDictionary<string, object>;
            
            if (dictObj != null)
            {
                object val;
                if (!dictObj.TryGetValue(field, out val))
                {
                    throw new ArgumentException(string.Concat(field, "不存在"));
                }

                return val;
            }

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

            var isArray = IsArray(fieldVal);
            if (isArray == false)
            {
                throw new ArgumentException("类型不匹配!");
            }

            var str = JsonConvert.SerializeObject(fieldVal);
            var newVal = JsonConvert.DeserializeObject(str, property.PropertyType);
            property.SetValue(data, newVal);
        }
    }
}
