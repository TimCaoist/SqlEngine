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

        public static object CreateInstance(string assemblyString, string typeStr)
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
            else {
                assembly = Assembly.Load(assemblyString);
                weakReference = new WeakReference<Assembly>(assembly);
                Assemblies.Add(assemblyString, weakReference);
            }

            return assembly.CreateInstance(typeStr);
        }
    }
}
