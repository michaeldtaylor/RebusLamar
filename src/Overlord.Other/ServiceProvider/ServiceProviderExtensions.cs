using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace Overlord.Other.ServiceProvider
{
    public static class ServiceProviderExtensions
    {
        public static void ScanWithDefaultConventions<T>(this IServiceCollection services)
        {
            services.Scan(s => s
                .FromAssemblyOf<T>()
                    .AddClasses()
                        .AsSelf()
                        .AsMatchingInterface()
                        .WithTransientLifetime());
        }

        public static void AddRegistry(this IServiceCollection services, Type type)
        {
            if (!typeof(ServiceRegistry).IsAssignableFrom(type))
            {
                throw new Exception($"Specified type is not a service registry. type={type.Name}");
            }

            var serviceRegistry = (ServiceRegistry)Activator.CreateInstance(type);

            serviceRegistry.Load(services);
        }

        public static void AddRegistry<TServiceRegistry>(this IServiceCollection services) where TServiceRegistry : ServiceRegistry, new()
        {
            services.AddRegistry(typeof(TServiceRegistry));
        }

        public static void AddRegistries<T>(this IServiceCollection services, Func<AssemblyName, bool> assemblyNameFilter = null)
        {
            var assemblies = AssemblyScanner.ScanTypeReferencedAssemblies<T>(assemblyNameFilter);
            var serviceRegistryTypes = new List<Type>();

            foreach (var assembly in assemblies)
            {
                serviceRegistryTypes.AddRange(assembly.GetTypes().Where(t => typeof(ServiceRegistry).IsAssignableFrom(t) && !t.IsAbstract));
            }

            foreach (var serviceRegistryType in serviceRegistryTypes)
            {
                services.AddRegistry(serviceRegistryType);
            }
        }
    }
}
