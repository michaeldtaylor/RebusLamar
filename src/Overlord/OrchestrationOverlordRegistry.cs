using Lamar;
using Microsoft.Extensions.DependencyInjection;
using Overlord.Infrastructure;

namespace Overlord
{
    public class OrchestrationOverlordRegistry : ServiceRegistry
    {
        public OrchestrationOverlordRegistry()
        {
            Scan(x =>
            {
                x.AssemblyContainingType<OrchestrationOverlordRegistry>();
                x.WithDefaultConventions();
            });

            this.AddTransient<IAppNameProvider, HostEnvironmentAppNameProvider>();
        }
    }
}
