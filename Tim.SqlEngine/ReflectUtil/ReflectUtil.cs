using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Tim.SqlEngine.ReflectUtil
{
    public static class ReflectUtil
    {
        private readonly static Dictionary<string, WeakReference<Assembly>> Assemblies = new Dictionary<string, WeakReference<Assembly>>();

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
            var ps = itemType.GetProperty(field);
            if (ps == null)
            {
                throw new ArgumentException(string.Concat(field, "不存在"));
            }

            return ps.GetValue(data);
        }
    }
}
