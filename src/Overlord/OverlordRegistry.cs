using Microsoft.Extensions.DependencyInjection;
using Overlord.Infrastructure;
using Overlord.Other.ServiceProvider;

namespace Overlord
{
    public class OverlordRegistry : ServiceRegistry
    {
        public override void Load(IServiceCollection services)
        {
            services.ScanWithDefaultConventions<OverlordRegistry>();

            services.AddTransient<IAppNameProvider, HostEnvironmentAppNameProvider>();
        }
    }
}
