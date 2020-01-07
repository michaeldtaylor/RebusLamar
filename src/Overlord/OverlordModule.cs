using Autofac;
using Microsoft.Extensions.Hosting;
using Overlord.Infrastructure;

namespace Overlord
{
    public class OverlordModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            // Scan(x =>
            // {
            //     x.AssemblyContainingType<OrchestrationOverlordRegistry>();
            //     x.WithDefaultConventions();
            // });

            builder.RegisterAssemblyTypes(typeof(OverlordModule).Assembly)
                .Where(t => !t.IsAssignableTo<IHostedService>())
                .AsSelf()
                .AsImplementedInterfaces();

            builder.RegisterType<HostEnvironmentAppNameProvider>().As<IAppNameProvider>();
        }
    }
}
