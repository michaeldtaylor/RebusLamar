using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;

namespace Overlord.Other.ServiceProvider
{
    public static class AssemblyScanner
    {
        private static readonly Func<AssemblyName, bool> NoFilter = a => true;

        public static IEnumerable<Assembly> ScanTypeReferencedAssemblies<T>(Func<AssemblyName, bool> assemblyNameFilter = null)
        {
            var assemblyMap = new Dictionary<string, Assembly>();
            var parentAssembly = typeof(T).Assembly;

            assemblyMap.Add(parentAssembly.GetName().Name, parentAssembly);

            ScanAssembly(parentAssembly, assemblyMap, assemblyNameFilter ?? NoFilter);

            return assemblyMap.Values;
        }

        private static void ScanAssembly(Assembly parentAssembly, IDictionary<string, Assembly> assemblyMap, Func<AssemblyName, bool> assemblyFilter)
        {
            var assemblyNames = parentAssembly.GetReferencedAssemblies().Where(assemblyFilter);

            foreach (var assemblyName in assemblyNames)
            {
                if (assemblyMap.ContainsKey(assemblyName.Name))
                {
                    continue;
                }

                var referencedAssembly = AssemblyLoadContext.Default.LoadFromAssemblyName(assemblyName);

                assemblyMap.Add(assemblyName.Name, referencedAssembly);

                ScanAssembly(referencedAssembly, assemblyMap, assemblyFilter);
            }
        }
    }
}
